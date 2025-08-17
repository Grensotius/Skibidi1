namespace Loki.Game
{
	/// <summary>
	/// Return values for ApplicationExitCode.
	/// </summary>
	public enum ApplicationExitCodes
	{
		/// <summary>No error.</summary>
		None = 0,

		/// <summary>There was a patch and DPB is not usable.</summary>
		UnsupportedClientVersion = 1,

		/// <summary>The offsets required to run the bot are not uploaded yet.</summary>
		OffsetsMissing = 2,


		/// <summary>DreamPoeBot is exiting to perform an update.</summary>
		Updating = 5,

		/// <summary>DreamPoeBot is restarting.</summary>
		Restarting = 6,

		/// <summary>Compile errors were encountered.</summary>
		CompileErrors = 7,

		/// <summary>The offsets required to run were not obtained due to an auth error.</summary>
		AuthError = 8,

		/// <summary>Load errors were encountered.</summary>
		LoadErrors = 9,

		/// <summary>An unknown error occurred, please check the logs.</summary>
		Unknown = 998,

		/// <summary>The program cannot run because prerequisites are missing.</summary>
		MissingPrerequisites = 999,
	}
}