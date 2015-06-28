using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Management.Automation;
using System.Threading.Tasks;

namespace Rwb.PowerShell
{
	public abstract class ProcessCmdletBase : PSCmdlet
	{
		private const string EnvironmentPath = "Path";
		private const string EnvironmentSystemDrive = "SystemDrive";
		private const string EnvironmentSystemRoot = "SystemRoot";
		private const string EnvironmentTemp = "TEMP";

		private readonly IFileSystem _fileSystem;

		[Parameter, ValidateNotNullOrEmpty]
		public string WorkingDirectory { get; set; }

		[Alias("RunAs")]
		[Parameter, ValidateNotNullOrEmpty]
		public PSCredential Credential { get; set; }

		[Parameter]
		public SwitchParameter PassThru { get; set; }

		[Parameter]
		public IDictionary Environment { get; set; }

		protected abstract string GetFileName();
		protected abstract IEnumerable<string> GetArgumentList();


		protected ProcessCmdletBase(IFileSystem fileSystem)
		{
			_fileSystem = Require.NotNull(fileSystem, nameof(fileSystem));
		}

		protected override void BeginProcessing()
		{
			var processStartInfo = SetupProcessStartInfo();

			var process = Process.Start(processStartInfo);
			Debug.Assert(process != null);

			if (PassThru.IsPresent)
				WriteObject(process);

			ForwardStandardOutputAndError(process).Wait();
			process.WaitForExit();
		}

		private ProcessStartInfo SetupProcessStartInfo()
		{
			var processStartInfo = new ProcessStartInfo
			{
				FileName = GetFileName(),
				CreateNoWindow = true,
				LoadUserProfile = false,
				UseShellExecute = false,
				ErrorDialog = false,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
			};

			CleanEnvironmentVariables(processStartInfo.EnvironmentVariables);
			FillEnvironmentVariables(processStartInfo.EnvironmentVariables);

			var argumentList = GetArgumentList();
			if (argumentList != null)
				processStartInfo.Arguments = string.Join(" ", argumentList);

			if (WorkingDirectory != null)
			{
				processStartInfo.WorkingDirectory = this.GetFileSystemProviderPathFromPSPath(WorkingDirectory);
				_fileSystem.AssertDirectoryExists(processStartInfo.WorkingDirectory);
			}
			else
			{
				var currentLocation = CurrentProviderLocation("FileSystem");
				processStartInfo.WorkingDirectory = currentLocation.ProviderPath;
			}

			if (Credential != null)
			{
				processStartInfo.UserName = Credential.UserName;
				processStartInfo.Password = Credential.Password;
			}

			return processStartInfo;
		}

		private static void CleanEnvironmentVariables(StringDictionary variables)
		{
			variables.Clear();
			variables[EnvironmentSystemDrive] = System.Environment.GetEnvironmentVariable(EnvironmentSystemDrive);
			variables[EnvironmentSystemRoot] = System.Environment.GetEnvironmentVariable(EnvironmentSystemRoot);
			variables[EnvironmentPath] = System.Environment.SystemDirectory;
			variables[EnvironmentTemp] = System.Environment.GetEnvironmentVariable(EnvironmentTemp);
		}

		private void FillEnvironmentVariables(StringDictionary variables)
		{
			if (Environment == null)
				return;

			foreach (var pair in EnumerateDictionaryParameter(Environment, nameof(Environment)))
			{
				if (string.Equals(EnvironmentPath, pair.Key, StringComparison.OrdinalIgnoreCase))
					FillPath(variables, pair.Value);
				else
					variables.Add(pair.Key, pair.Value.ToString());
			}
		}

		/// <summary>
		/// Enumerate a IDictionary parameter (intended to accept PowerShell's Hashtable),
		/// ensure keys and values are not null,
		/// and converting keys to string
		/// </summary>
		protected static IEnumerable<KeyValuePair<string, object>> EnumerateDictionaryParameter(IDictionary dictionary, string parameterName)
		{
			foreach (var key in dictionary.Keys)
			{
				if (key == null)
					throw new ArgumentException(@"A null key is not allowed the dictionary", parameterName);

				object value = dictionary[key];
				if (value == null)
					throw new ArgumentException("Dictionary entry with key \"{0}\" is null".FormatInvariant(key), parameterName);

				string keyStr = key.ToString();

				yield return new KeyValuePair<string, object>(keyStr, value);
			}
		}

		private static void FillPath(StringDictionary variables, object value)
		{
			if (!(value is string) && value is IEnumerable<object>)
				value = string.Join(";", value);

			variables[EnvironmentPath] = variables[EnvironmentPath] + ";" + value;
		}

		private Task ForwardStandardOutputAndError(Process process)
		{
			var standardOutputTask = Task.Run(async () =>
			{
				string line;
				while ((line = await process.StandardOutput.ReadLineAsync()) != null)
					Host.UI.WriteLine(line);
			});
			var standardErrorTask = Task.Run(async () =>
			{
				string line;
				while ((line = await process.StandardError.ReadLineAsync()) != null)
					Host.UI.WriteErrorLine(line);
			});

			return Task.WhenAll(standardOutputTask, standardErrorTask);
		}
	}
}
