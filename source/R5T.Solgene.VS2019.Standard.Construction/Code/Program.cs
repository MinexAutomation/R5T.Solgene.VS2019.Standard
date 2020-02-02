﻿using System;

using Microsoft.Extensions.DependencyInjection;

using R5T.Bath;
using R5T.Bedford;
using R5T.Cambridge.Types;
using R5T.Chalandri;
using R5T.Evosmos;
using R5T.Ilioupoli.Default;
using R5T.Richmond;
using R5T.Solutas;


namespace R5T.Solgene.VS2019.Standard.Construction
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var serviceProvider = ServiceProviderBuilder.NewUseStartup<Startup>() as ServiceProvider)
            {
                var program = serviceProvider.GetRequiredService<Program>();

                program.Run();
            }
        }


        private IVisualStudioSolutionFileGenerator VisualStudioSolutionFileGenerator { get; }
        private ITemporaryDirectoryFilePathProvider TemporaryDirectoryFilePathProvider { get; }
        private IVisualStudioSolutionFileSerializer VisualStudioSolutionFileSerializer { get; }
        private ITestingDataDirectoryContentPathsProvider TestingDataDirectoryContentPathsProvider { get; }
        private IFileEqualityComparer FileEqualityComparer { get; }
        private IHumanOutput HumanOutput { get; }


        public Program(
            IVisualStudioSolutionFileGenerator visualStudioSolutionFileGenerator,
            ITemporaryDirectoryFilePathProvider temporaryDirectoryFilePathProvider,
            IVisualStudioSolutionFileSerializer visualStudioSolutionFileSerializer,
            ITestingDataDirectoryContentPathsProvider testingDataDirectoryContentPathsProvider,
            IFileEqualityComparer fileEqualityComparer,
            IHumanOutput humanOutput
            )
        {
            this.VisualStudioSolutionFileGenerator = visualStudioSolutionFileGenerator;
            this.TemporaryDirectoryFilePathProvider = temporaryDirectoryFilePathProvider;
            this.VisualStudioSolutionFileSerializer = visualStudioSolutionFileSerializer;
            this.TestingDataDirectoryContentPathsProvider = testingDataDirectoryContentPathsProvider;
            this.FileEqualityComparer = fileEqualityComparer;
            this.HumanOutput = humanOutput;
        }

        public void Run()
        {
            var newSolutionFile = this.VisualStudioSolutionFileGenerator.GenerateVisualStudioSolutionFile();

            // Changes to make generated solution file match specific example solution file.
            newSolutionFile.VisualStudioVersion = Version.Parse("16.0.29613.14");
            newSolutionFile.MinimumVisualStudioVersion = Version.Parse("10.0.40219.1");

            var extensibilityGlobals = newSolutionFile.GlobalSections.GetExtensibilityGlobalsGlobalSection();
            extensibilityGlobals.SolutionGuid = Guid.Parse("5612D14F-4C73-4FAD-A249-631DF0E3D8F1");

            // Now serialize.
            var serializationFilePath = this.TemporaryDirectoryFilePathProvider.GetTemporaryDirectoryFilePath(TestingDataDirectoryContentConventions.NewVisualStudio2019SolutionFileNameValue);

            this.VisualStudioSolutionFileSerializer.Serialize(serializationFilePath, newSolutionFile);

            var testingFilePath = this.TestingDataDirectoryContentPathsProvider.GetNewVisualStudio2019SolutionFilePath();

            var isEqual = this.FileEqualityComparer.Equals(serializationFilePath, testingFilePath);

            if (isEqual)
            {
                this.HumanOutput.WriteLine("New VS2019 solution file equals the testing VS2019 solution file.");
            }
            else
            {
                this.HumanOutput.WriteLine("New VS2019 solution file did NOT equal the testing VS2019 solution file...");
            }
        }
    }
}
