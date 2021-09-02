# MMDHelpers.CSharp

### This project is a collection of helpers that i've been using across multiple projects
### Feedbacks are always welcome :D
Some of the code are production ready, and some are just experimentations

## Projects and purpose

- [Nuget] MMDHelpers.CSharp - simple helpers, you should avoid them if your application needs to be high performance.
- [Nuget] MMDHelpers.CSharp.LDAP - simple abstraction to login on a LDAP service.
- [Nuget] MMDHelpers.CSharp.LocalData - generates a localDB, uses dapper as micro-orm.
- [Nuget] MMDHelpers.CSharp.Performance.Grpc - An example to use Grpc to measure Performance (not ideal, but just enough to get the evident ones)
- MMDHelpers.CSharp.Performance.GrpcClient - A client to control the server.
- [Nuget] MMDHelpers.CSharp.PerformanceChecks - Just shows the GC Collection and workingSet being used.


### MMDHelpers.CSharp


#### WriteToFile
As i mentioned before, there are better solutions like [Log4net](https://logging.apache.org/log4net/), [Serilog](https://serilog.net/), [NLog](https://nlog-project.org/)
but for small projects or just to Test this is enough.
``` Csharp
        ////An Example on Application
        public static void Write(string filename, params string[] texts)
        {
            if (texts.Length == 0) return;
            lock (sync)
            {
                filename
                    .ToCurrentPath()
                    .WriteToFile(new List<string>(texts.Length) { string.Join("", texts.Select(c => $"{DateTime.Now} - {c} {Environment.NewLine}")) });
            }
        }
        
        ////Usage:
        Write("somefile.log","test 1", "test 2");
```
#### AsDataTable
As the Example Above this will be enough when your collection is small around 10-150k. 
beware it'll have overhead, as internally i'm using reflection to get types.
``` CSharp
public void SomeMethodToBulkInsert<T>(IEnumerable<T> somelist )
using (var conn = new SqlConnection("someconn"))
{
    conn.Open();
    var transaction = conn.BeginTransaction();
    using (var bulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, transaction))
    {
        bulkCopy.BatchSize = 500;
        bulkCopy.DestinationTableName = "dbo.someTable";
        try
        {
            SqlBulkCopyColumnMapping someColumn = new SqlBulkCopyColumnMapping("source", "destination");
            
            bulkCopy.ColumnMappings.Add(someColumn);
            
            bulkCopy.WriteToServer(somelist.Select(q => new { q.someColumn }).AsDataTable());
            transaction.Commit();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
        }
    }
}
```

## MMDHelpers.CSharp.PerformanceChecks
If you need high performant tools to analyze your code, i recommend my [Book of Reference](https://github.com/millerscout/book-of-reference/blob/master/CSharp.md).
there i'm collecting Articles, tools to those scenarios.
``` CSharp
    Ruler.StartMeasuring(false);
    Ruler.StopMeasuring(false);
    Ruler.Show(true);
    Ruler.LogToFile();
```

## MMDHelpers.CSharp.Performance.Grpc

I've written this code to experiment how Grpc works, 


``` CSharp
	//Configuring on DependencyInjection:
	public void ConfigureServices(IServiceCollection services)
            {
                services.AddGrpc();
            }
            public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
            {
				(...)
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapGrpcService<LogService>();
                });
	}
	
	//If you're not using Kestrel, i've implemented a simple one,  this should be enough to test.
    _ = Task.Run(()=>new MMDHelpers.CSharp.Performance.grpc.LocalKestrelServer().Run());

```

## MMDHelpers.CSharp.Performance.GrpcClient

simple console that sends instructions to server, it'll 
start, stop measurement, get log and write to file.

## MMDHelpers.CSharp.Performance.BufferedProcess

Across some projects i had to bufferize a collection, that way i would use the same objects over and over again, without needing to allocate more memory.



``` CSharp
	
	// My Latest Context i've connected to a Queue and the messages were indexed or ignored .
	// the ones that were indexed i had to process and insert into a database.
	// this code should be enough to give you the head-start
	
	
	
	
	//start code
	/// how many items inside a collection.
	var  perBuffer = 1000;
	/// how many collections will be created
	var collections = 2;
	var bF = new BufferedProcess<someclass>(Convert.ToUInt32(perBuffer), collections);

	/// when maximum items are filled this event will be raised.
	bF.onBufferReached += BF_onBufferReached;
	
	
	/// code while reading a source
	bF.SelectBufferReturnsIndexItem();

	if (bF.bufferedList[bF.currentIndexBuffer][bF.CurrentItemInBufer]== null)
		bF.bufferedList[bF.currentIndexBuffer][bF.CurrentItemInBufer] = new someclass(){someValue = 0 }
	}else{
		bF.bufferedList[bF.currentIndexBuffer][bF.CurrentItemInBufer].someValue = 0;
	}
	
	///
	
	
	public class someclass(){
		public int someValue {get;set;}
	}
```

