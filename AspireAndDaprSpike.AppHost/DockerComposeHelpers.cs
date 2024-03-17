using System.Diagnostics;

namespace AspireAndDaprSpike.AppHost
{
    public static class DockerComposeHelper
    {
        public static void StartDockerCompose(bool removeVolumes)
        {
            // Run "docker compose down" with or without the "-v" flag based on configuration
            if (removeVolumes)
            {
                RunDockerComposeCommand("down -v");
            }

            // Run "docker compose up -d" to start the containers
            RunDockerComposeCommand("up -d");
        }

        private static void RunDockerComposeCommand(string arguments)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo("docker-compose")
            {
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
            };

            using Process process = new Process { StartInfo = startInfo };
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (!string.IsNullOrEmpty(error))
            {
                Console.WriteLine($"Error running 'docker-compose {arguments}': {error}");
            }
            else
            {
                Console.WriteLine($"'docker-compose {arguments}' executed successfully.");
            }
        }
    }
}
