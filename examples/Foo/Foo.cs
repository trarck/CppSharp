using System;
using CppSharp.AST;
using CppSharp.Generators;
using CppSharp.Passes;
using System.Text;
using System.IO;

namespace CppSharp
{
    class FooLibrary: ILibrary
    {
        readonly string name;
        readonly GeneratorKind kind;

        protected FooLibrary(string name, GeneratorKind kind)
        {
            this.name = name;
            this.kind = kind;
        }

        public virtual void Setup(Driver driver)
        {
            var options = driver.Options;
            options.LibraryName = name;
            options.GeneratorKind = kind;
            options.OutputDir = Path.Combine(GetOutputDirectory(), "gen", name);
            options.SharedLibraryName = name + ".Native";
            options.GenerateLibraryNamespace = true;
            options.Quiet = true;
            options.IgnoreParseWarnings = true;

            driver.Diagnostics.Message("");
            driver.Diagnostics.Message("Generating bindings for {0} ({1})",
                options.LibraryName, options.GeneratorKind.ToString());


            var path = Path.GetFullPath(GetTestsDirectory(name));
            options.addIncludeDirs(path);

            driver.Diagnostics.Message("Looking for tests in: {0}", path);
            var files = Directory.EnumerateFiles(path, "*.h");
            foreach (var file in files)
                options.Headers.Add(Path.GetFileName(file));
        }

        public void Postprocess(Driver driver, ASTContext ctx)
        {
            Console.WriteLine("Postprocess");
        }

        public void Preprocess(Driver driver, ASTContext ctx)
        {
            Console.WriteLine("Preprocess");
        }

        public void SetupPasses(Driver driver)
        {
            driver.Options.Encoding = Encoding.Unicode;
        }

        static string GetOutputDirectory()
        {
            var directory = Directory.GetParent(Directory.GetCurrentDirectory());

            while (directory != null)
            {
                var path = Path.Combine(directory.FullName, "obj");

                if (Directory.Exists(path))
                    return directory.FullName;

                directory = directory.Parent;
            }

            throw new Exception("Could not find tests output directory");
        }

        public static string GetTestsDirectory(string name)
        {
            var directory = Directory.GetParent(Directory.GetCurrentDirectory());

            while (directory != null)
            {
                var path = Path.Combine(directory.FullName, "examples", name);

                if (Directory.Exists(path))
                    return path;

                directory = directory.Parent;
            }

            throw new Exception(string.Format(
                "Tests directory for project '{0}' was not found", name));
        }

        static class Program
        {
            public static void Main(string[] args)
            {
                ConsoleDriver.Run(new FooLibrary("Foo",GeneratorKind.CSharp));
                Console.Read();
            }
        }
    }
}
