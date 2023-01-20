namespace _Scripts.System
{
	public partial class DataManager : qtSingleton<DataManager>
	{
		#region ----- KEY PATH -----

		#endregion

		#region ----- VARIABLE -----


		#endregion

		#region ----- INITIALIZE -----

		protected override void Init()
		{
			base.Init();
			Initialize();
			DontDestroyOnLoad(this);
		}

		public void Initialize()
		{
		}

		#endregion

		#region ----- PUBLIC FUNCTION -----


		#endregion

		#region ----- PRIVATE FUNCTION -----

		
		#endregion

		#region ----- PUBLIC FUNCTION -----

		
		#endregion
	}
}