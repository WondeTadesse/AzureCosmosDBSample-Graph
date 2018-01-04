//|---------------------------------------------------------------|
//|                         AZURE GRAPH DB                        |
//|---------------------------------------------------------------|
//|                       Developed by Wonde Tadesse              |
//|                             Copyright ©2018 - Present         |
//|---------------------------------------------------------------|
//|                         AZURE GRAPH DB                        |
//|---------------------------------------------------------------|
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.Graphs;
using Newtonsoft.Json;

namespace AzureGraphDBSample
{
    public class GremlinEmployeeGraphProcessor
    {

        #region Public Methods 

        /// <summary>
        /// Process employee graph
        /// </summary>
        public void ProcessEmployeeGraph()
        {
            DocumentClient documentClient;
            if (TryCreatingDocumentClient(out documentClient))
            {
                if (TryCreatingGraphDatabase(documentClient))
                {
                    DocumentCollection graph;
                    if (TryCreatingGraph(documentClient, out graph))
                    {
                        if (Add3EmployeeVertexToGraph(documentClient, graph))
                        {
                            if (Add2EmployeeEdgeToGraph(documentClient, graph))
                            {

                                Console.WriteLine("Processing employee graph completed !\n");
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Private Methods 

        /// <summary>
        /// Try creating document client
        /// </summary>
        /// <param name="documentClient">DocumentClient object</param>
        /// <returns>true/false</returns>
        private bool TryCreatingDocumentClient(out DocumentClient documentClient)
        {
            bool isDocumentClientCreated = false;
            Console.ForegroundColor = ConsoleColor.Green;
            documentClient = null;
            try
            {
                documentClient = new DocumentClient(
                    new Uri(ConfigurationManager.AppSettings.Get("graphAPIURI")),
                    ConfigurationManager.AppSettings.Get("graphAPIAuthKey"),
                    new ConnectionPolicy
                    {
                        ConnectionMode = ConnectionMode.Direct,
                        ConnectionProtocol = Protocol.Tcp
                    });
                isDocumentClientCreated = true;
                Console.WriteLine("Graph document client successfully created !\n");
            }
            catch (Exception exception)
            {
                isDocumentClientCreated = false;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error Occurred !");
                Console.WriteLine(exception);
                Console.ForegroundColor = ConsoleColor.Green;
            }
            return isDocumentClientCreated;
        }

        /// <summary>
        /// Trt to create graph database
        /// </summary>
        /// <param name="documentClient">DocumentClient object</param>
        /// <returns>true/false</returns>
        private bool TryCreatingGraphDatabase(DocumentClient documentClient)
        {
            bool isGraphDatabaseCreated = false;
            try
            {
                Database database = documentClient.CreateDatabaseIfNotExistsAsync(new Database
                {
                    Id = ConfigurationManager.AppSettings.Get("graphDatabaseName")
                }).Result;
                isGraphDatabaseCreated = true;
                Console.WriteLine($"Graph database [{database.Id}] successfully created !\n");
            }
            catch (Exception exception)
            {
                isGraphDatabaseCreated = false;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error Occurred !");
                Console.WriteLine(exception);
                Console.ForegroundColor = ConsoleColor.Green;
            }
            return isGraphDatabaseCreated;
        }

        /// <summary>
        /// Try creating graph
        /// </summary>
        /// <param name="documentClient"></param>
        /// <param name="graph"></param>
        /// <returns></returns>
        private bool TryCreatingGraph(DocumentClient documentClient, out DocumentCollection graph)
        {
            bool isGraphCreated = false;
            graph = null;
            try
            {
                graph = documentClient.CreateDocumentCollectionIfNotExistsAsync(
                UriFactory.CreateDatabaseUri(ConfigurationManager.AppSettings.Get("graphDatabaseName")),
                    new DocumentCollection
                    {
                        Id = ConfigurationManager.AppSettings.Get("graphName"),
                    },
                    new RequestOptions
                    {
                        OfferThroughput = 400
                    }).Result;
                isGraphCreated = true;
                Console.WriteLine($"Graph database [{graph.Id}] successfully created !\n");
            }
            catch (Exception exception)
            {
                isGraphCreated = false;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error Occurred !");
                Console.WriteLine(exception);
                Console.ForegroundColor = ConsoleColor.Green;
            }
            return isGraphCreated;
        }

        /// <summary>
        /// Add three employee to the graph
        /// </summary>
        /// <param name="documentClient">DocumentClient object</param>
        /// <param name="graph">Graph object</param>
        /// <returns>true/false</returns>
        private bool Add3EmployeeVertexToGraph(DocumentClient documentClient, DocumentCollection graph)
        {
            bool isEmployeeVertexAdded = false;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("About to add three employee to the graph !\n");
            Thread.Sleep(2000);
            try
            {
                Dictionary<string, string> gremlinQueries = new Dictionary<string, string>
                {
                    { "Cleanup",     "g.V().drop()" },
                    { "Mike Vertex", "g.addV('employee').property('id', 'Mike').property('firstName', 'Mike').property('lastName', 'M').property('jobTitle', 'Quality Assurance')" },
                    { "Jane Vertex", "g.addV('employee').property('id', 'Jane').property('firstName', 'Jane').property('lastName', 'L').property('jobTitle', 'Software Engineer')" },
                    { "Tom Vertex",  "g.addV('employee').property('id', 'Tom').property('firstName', 'Tom').property('lastName', 'S').property('jobTitle', 'Manager')" }
                };

                foreach (KeyValuePair<string, string> gremlinQuery in gremlinQueries)
                {
                    IDocumentQuery<dynamic> query = documentClient.CreateGremlinQuery<dynamic>(graph, gremlinQuery.Value);
                    while (query.HasMoreResults)
                    {
                        foreach (dynamic result in query.ExecuteNextAsync().Result)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"{JsonConvert.SerializeObject(result, Formatting.Indented)}\n");
                            Console.ForegroundColor = ConsoleColor.Green;
                            Thread.Sleep(2000); // Wait 2 seconds to process the next query
                        }
                    }
                    Console.WriteLine();
                }
                isEmployeeVertexAdded = true;
                Console.WriteLine("Three employees are successfully added to the graph !\n");
            }
            catch (Exception exception)
            {
                isEmployeeVertexAdded = false;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error Occurred !");
                Console.WriteLine(exception);
                Console.ForegroundColor = ConsoleColor.Green;
            }
            return isEmployeeVertexAdded;
        }

        /// <summary>
        /// Add two employee edge
        /// </summary>
        /// <param name="documentClient">DocumentClient object</param>
        /// <param name="graph">Graph object</param>
        /// <returns>true/false</returns>
        private bool Add2EmployeeEdgeToGraph(DocumentClient documentClient, DocumentCollection graph)
        {
            bool isEmployeeEdgeAdded = false;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("About to add two employee edge named 'works for' to the graph !\n");
            Thread.Sleep(2000);
            try
            {
                Dictionary<string, string> gremlinQueries = new Dictionary<string, string>
                {
                    { "Mike works for Tom edge", "g.V('Mike').addE('works for').to(g.V('Tom'))" },
                    { "Jane works for Tom edge", "g.V('Jane').addE('works for').to(g.V('Tom'))" },
                };

                foreach (KeyValuePair<string, string> gremlinQuery in gremlinQueries)
                {
                    Console.WriteLine($"About to run [{gremlinQuery.Key}] graph query !\n");
                    IDocumentQuery<dynamic> query = documentClient.CreateGremlinQuery<dynamic>(graph, gremlinQuery.Value);
                    while (query.HasMoreResults)
                    {
                        var resultset = query.ExecuteNextAsync().Result;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"[{gremlinQuery.Key}] is created !\n");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Thread.Sleep(2000); // Wait 2 seconds to process the next query
                    }
                }
                Console.WriteLine("Two employee edge named 'works for' successfully added to the graph !\n");
                isEmployeeEdgeAdded = true;
            }
            catch (Exception exception)
            {
                isEmployeeEdgeAdded = false;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error Occurred !");
                Console.WriteLine(exception);
                Console.ForegroundColor = ConsoleColor.Green;
            }
            return isEmployeeEdgeAdded;
        }

        #endregion
    }
}
