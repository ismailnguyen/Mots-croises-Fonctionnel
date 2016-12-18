open System
open System.Net
open System.Text
open System.IO

 // interactive mode then go to http://localhost:9000/mots-croises.json in your favorite browser

 // Let's see https://msdn.microsoft.com/en-us/library/system.net.httplistenerrequest(v=vs.110).aspx

let siteRoot = @"C:\Users\MonCompte\Desktop\Mots-croises-Fonctionnel\"
let host = "http://localhost:9000/"
 
let listener (handler:(HttpListenerRequest->HttpListenerResponse->Async<unit>)) =
    let hl = new HttpListener()
    hl.Prefixes.Add host
    hl.Start()
    let task = Async.FromBeginEnd(hl.BeginGetContext, hl.EndGetContext)
    async {
        while true do
            let! context = task
            Async.Start(handler context.Request context.Response)
    } |> Async.Start
 
let output (req:HttpListenerRequest) =
    let file = Path.Combine(siteRoot,
                            Uri(host).MakeRelativeUri(req.Url).OriginalString)
    printfn "Requested : '%s'" file
    Console.WriteLine("HTTP method: {0}", req.HttpMethod);
    if (File.Exists file)
        then File.ReadAllText(file)
        else "File does not exist!"
 
listener (fun req resp ->
    async {
        let txt = Encoding.ASCII.GetBytes(output req)
        resp.ContentType <- "text/html"
        resp.OutputStream.Write(txt, 0, txt.Length)
        resp.OutputStream.Close()
    })

// TODO: add your code here

// TODO: Create a function which interact in function of the case of the req method (GET, PUT, POST, DELETE)
// Maybe we just need GET method because if we just check the map and the response it's enough ?