// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using System.Text.Json.Serialization;
using FirstGrpc;
using Grpc.Net.Client;

Console.WriteLine("Hello, World!");

var channel = GrpcChannel.ForAddress("http://localhost:5046");
var grpcClient = new PersonService.PersonServiceClient(channel);
grpcClient.AddPerson(new CreatePersonRequest
{
    Family = "shokri",
    Name = "Navid",
    BirthDay = Convert.ToUInt64(new DateTime(1988,9,19).Ticks)
});

grpcClient.AddPerson(new CreatePersonRequest
{
    Family = "shokri",
    Name = "Behrad",
    BirthDay = Convert.ToUInt64(new DateTime(2019,7,9).Ticks)
});

grpcClient.AddPerson(new CreatePersonRequest
{
    Family = "Mahdavi",
    Name = "Reyhane",
    BirthDay = Convert.ToUInt64(new DateTime(1988,6,25).Ticks)
});

var list = grpcClient.GetAllPerson(new GetAllRequest()); 

Console.WriteLine(JsonSerializer.Serialize(list));