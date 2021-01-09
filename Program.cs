using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using Shimakaze.Struct.Csf;
using Shimakaze.Struct.Csf.Json;

namespace Shimakaze.Toolkit.CsfJsonConverter
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length > 0)
            {
                Func<Stream, Stream, Task> method;
                string extension;
                if (args[0].EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                {
                    method = Json2Csf;
                    extension = ".csf";
                }
                else if (args[0].EndsWith(".csf", StringComparison.OrdinalIgnoreCase))
                {
                    method = Csf2Json;
                    extension = ".json";
                }
                else
                    throw new NotSupportedException();

                await using var input = File.OpenRead(args[0]);
                await using var output = File.OpenRead(args.Length > 1 ? args[1] : Path.GetFileNameWithoutExtension(args[0]) + extension);
                await method(input, output);
            }
            else
            {
                Func<Stream, Stream, Task> method;
                string extension;
                Console.Write("Input File: ");
                var inputPath = Console.ReadLine().Trim('\'', '\"');
                if (inputPath.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                {
                    method = Json2Csf;
                    extension = ".csf";
                }
                else if (inputPath.EndsWith(".csf", StringComparison.OrdinalIgnoreCase))
                {
                    method = Csf2Json;
                    extension = ".json";
                }
                else
                    throw new NotSupportedException();
                Console.Write("Output File: ");
                var outputPath = Console.ReadLine().Trim('\'', '\"');
                if (string.IsNullOrWhiteSpace(outputPath))
                {
                    outputPath = Path.GetFileNameWithoutExtension(args[0]) + extension;
                    Console.CursorTop--;
                    Console.Write($"Output File: {outputPath}");
                }


                await using var input = File.OpenRead(inputPath);
                await using var output = File.OpenRead(outputPath);
                await method(input, output);
            }
        }

        static Task Csf2Json(Stream input, Stream output)
            => JsonSerializer.SerializeAsync(output, CsfStructUtils.Serialize(input), CsfJsonConverterUtils.CsfJsonSerializerOptions);

        static async Task Json2Csf(Stream input, Stream output)
        {
            var p = await JsonSerializer.DeserializeAsync<CsfStruct>(input, CsfJsonConverterUtils.CsfJsonSerializerOptions);
            p.Deserialize(output);
        }
    }
}
