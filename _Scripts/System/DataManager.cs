using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using _Prefab.Popup;
using _Scripts.RrpcRequest;
using GFramework;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using MasterData;
using UnityEngine;
using Newtonsoft.Json;
using UnityCipher;
namespace _Scripts.System
{
	public partial class DataManager : qtSingleton<DataManager>
	{
		#region ----- KEY PATH -----

		private const string GameData = "/GameData.dat";
		private const string PASSWORD = "r3rW36tR%kRdz@?S";

		#endregion

		#region ----- VARIABLE -----

		private ItemDecorData _itemDecorData;

		public ItemDecorData itemDecorData
		{
			get
			{
				if (_itemDecorData == null)
				{
					_itemDecorData = Resources.Load<ItemDecorData>("_Data/ItemDecorData");
				}

				return _itemDecorData;
			}
		}

		public int Weight => 3; //1 CheckVersion + 1 LoadMasterData + 1 SaveData
		private int _wCounter = 0;

		private static readonly Dictionary<Type, Dictionary<long, IMessage>> MDataCache =
			new Dictionary<Type, Dictionary<long, IMessage>>();
		
		private long _currentDataVersion = -1;
		private MDataDownloadState _mDataDownloadState;
		
		public MenuScreenDecor menuScreenDecor { get; private set; }
		
		
		private enum MDataDownloadState
		{
			None,
			CheckVersionMData,
			DownloadMData,
			SaveData,
			End
		}

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
			menuScreenDecor ??= new MenuScreenDecor();
		}

		#endregion

		#region ----- PUBLIC FUNCTION -----

		public bool RegisterNewUser(string username, string password)
		{
			SaveData();
			return true;
		}

		#endregion

		#region ----- PRIVATE FUNCTION -----

		private void SaveData()
		{
		}
		
		private void CheckMDataVersion(Action<long> onComplete = null)
		{
			CheckVersionDataRequest checkVersionDataRequest = new CheckVersionDataRequest();
			checkVersionDataRequest.Request<CheckVersionReply>(res =>
			{
				onComplete?.Invoke(res.CurrentVersion);
			});
		}

		private void DownloadMData(Action onFinish = null)
		{
			DownloadDataRequest downloadDataRequest = new DownloadDataRequest();
			downloadDataRequest.Request<MasterDataReply>(masterDataReply =>
			{
				var splitNames = masterDataReply.Data.TypeUrl.Split("/");
				var messageDescriptor = GetMessageDescriptor(splitNames[^1]);
				var iMessage = messageDescriptor.Parser.ParseFrom(masterDataReply.Data.Value);
				Add(iMessage);
			}, null, onFinish);
		}
		#endregion

		#region ----- PUBLIC FUNCTION -----

		#region DownloadMasterData
		public void DownloadDataFromServer(Action<float, float> process = null, Action onComplete = null)
		{
			_mDataDownloadState = MDataDownloadState.None;
			SetState(MDataDownloadState.CheckVersionMData);
			
			void SetState(MDataDownloadState downloadState)
			{
				_mDataDownloadState = downloadState;
				
				Logger.Log("DowloadDataState: " + _mDataDownloadState);
			
				switch (_mDataDownloadState)
				{
					case MDataDownloadState.CheckVersionMData:
					{	
						process?.Invoke(_wCounter, Weight);
						CheckMDataVersion(currentVersion =>
						{
							process?.Invoke(++_wCounter, Weight);
							if (currentVersion > long.Parse(PlayerPrefs.GetString(KeyConstraint.LastVersionData, "-1")))
							{
								_currentDataVersion = currentVersion;
								SetState(MDataDownloadState.DownloadMData);
							}
							else
							{
								SetState(MDataDownloadState.End);
							}
						});
						break;
					}
					case MDataDownloadState.DownloadMData:
					{
						StartCoroutine(DownloadProcess());
						IEnumerator DownloadProcess()
						{
							for (int i = 0; i < 1 * 100; i++)
							{
								yield return new WaitForSeconds(0.01f);
								process?.Invoke(_wCounter + i / 100, Weight);
							}
						}

						DownloadMData(() => {
							StopCoroutine(DownloadProcess());
							process?.Invoke(++_wCounter, Weight);
							SetState(MDataDownloadState.SaveData);
						});
						break;
					}
					case MDataDownloadState.SaveData:
					{
						SaveMasterDataToDisk(_wCounter, process, () =>
						{
							PlayerPrefs.SetString(KeyConstraint.LastVersionData, _currentDataVersion.ToString());
							SetState(MDataDownloadState.End);
						});
						break;
					}
					case MDataDownloadState.End:
					{
						onComplete?.Invoke();
						break;
					}
				}
			}
		}

		public void SaveMasterDataToDisk(int wCurrent, Action<float, float> process = null, Action onFinish = null)
		{
			StartCoroutine(SaveData());

			IEnumerator SaveData()
			{
				var folderPath = LocationManager.GetMasterDataFolderPath();
					
				if (FileManager.IsDirectoryExist(folderPath))
				{
					FileManager.ClearDirectory(folderPath);
				}

				int counter = 0;
				foreach (var masterdata in MDataCache)
				{
					var type = masterdata.Key;
					var name = type.Name;
					var path = LocationManager.GetMasterDataFilePath(name);
					var list = MDataCache[type].Values.ToList();
					var data = list.Select(JsonFormatter.ToDiagnosticString).ToList();
					var jsonData = JsonConvert.SerializeObject(data);
					var byteData = Encoding.UTF8.GetBytes(jsonData);
					var encryptedData = RijndaelEncryption.Encrypt(byteData, PASSWORD);

					if (FileManager.WriteFile(path, encryptedData) == false)
					{
						var isWait = true;

						UIManager.Instance.ShowPopup<NotiPopup>(qtScene.EPopup.Noti).Initialize("",
							"NOT_ENOUGH_STORAGE",
							() => { isWait = false; });
						yield return new WaitWhile(() => isWait);
					}
					else
					{
						counter++;
						process?.Invoke(wCurrent + counter / MDataCache.Count, Weight);
					}
				}

				onFinish?.Invoke();
			}
		}
		
		public MessageDescriptor GetMessageDescriptor(string dataName)
		{
			foreach (var messageType in MasterDataReflection.Descriptor.MessageTypes)
			{
				if (messageType.FullName == dataName)
				{
					return messageType;
				}
			}
			return null;
		}
		#endregion

		#region LoadDataFromLocal
		public void LoadDataFromDisk(Action<int, int> process = null, Action onComplete = null)
		{
			int typesCount = MasterDataReflection.Descriptor.MessageTypes.Count;
			
			/*if (MDataCache.Count > 0)
			{
				process?.Invoke(weight, weight);
				onComplete?.Invoke();
				return;
			}*/
			
			MDataCache.Clear();

			IEnumerator IELoadFromDisk()
			{
				for (var index = 0; index < typesCount; index++)
				{
					var descriptor = MasterDataReflection.Descriptor.MessageTypes[index];
					var name = descriptor.Name;
					var path = LocationManager.GetMasterDataFilePath(name);

					var isWait = true;
					FileManager.ReadFile(path, unityWebRequest =>
					{
						var rawData = unityWebRequest.downloadHandler.data;
						var decryptedData = RijndaelEncryption.Decrypt(rawData, PASSWORD);
						var jsonData = Encoding.UTF8.GetString(decryptedData);
						var dataList = JsonConvert.DeserializeObject<List<string>>(jsonData);

						var list = new List<IMessage>();
						foreach (var data in dataList)
						{
							list.Add(descriptor.Parser.ParseJson(data));
						}

						Add(list);

						isWait = false;
					}, () => { isWait = false; });

					yield return new WaitWhile(() => isWait);
					process?.Invoke(index + 1, typesCount);
				}

				onComplete?.Invoke();
			}

			GMonoBehaviourUtility.Instance.StartCoroutine(IELoadFromDisk());
		}
		#endregion

		#region Get/Add Data
public void Add(IMessage data, bool isNewData = false)
		{
			if (data == null) return;

			var type = data.GetType();
			if (!MDataCache.ContainsKey(type))
			{
				MDataCache.Add(type, new Dictionary<long, IMessage>());
			}

			Logger.Log("Model: " + type);
			var descriptor = data.Descriptor;
			var id = (long) descriptor.FindFieldByName("id").Accessor.GetValue(data);
			
			MDataCache[type][id] = data;
			
		}

		public void Add(List<IMessage> list)
		{
			foreach (var data in list)
			{
				Add(data);
			}
		}

		public void Add<T>(T data) where T : IMessage, new()
		{
			var type = typeof(T);
			if (!MDataCache.ContainsKey(type))
			{
				MDataCache.Add(type, new Dictionary<long, IMessage>());
			}

			if (data != null)
			{
				var descriptor = data.Descriptor;
				var id = (long) descriptor.FindFieldByName("id").Accessor.GetValue(data);

				MDataCache[type][id] = data;
			}
			
		}
		
		private bool TryGetFromCache<T>(long id, out T result) where T : class, IMessage, new()
		{
			var type = typeof(T);
			result = default;
			
			if (MDataCache.ContainsKey(type) == false) return false;
			if (MDataCache[type].ContainsKey(id) == false) return false;

			result = MDataCache[type][id] as T;

			return true;
		}
		
		public T Get<T>(long id) where T: class, IMessage, new () 
	    {
	        var type = typeof(T);
	        if (MDataCache.ContainsKey(type))
	        {
		        if (MDataCache[type].ContainsKey(id)) return MDataCache[type][id] as T;
	        }
        
	        return new T();
	    }
    
	    public bool TryToGet<T>(long id, out T result) where T: class, IMessage, new () 
	    {
		    var type = typeof(T);
		    if (type == typeof(ResourceProto))
		    {
			    
		    }
		    if (MDataCache.ContainsKey(type))
		    {
			    if (MDataCache[type].ContainsKey(id))
			    {
				    result = MDataCache[type][id] as T;
				    return true;
			    }
		    }

		    result = default;
		    return false;
	    }

	    public void Get<T>(long id, Action<T> callback) where T : class, IMessage, new()
	    {
		    if (TryGetFromCache<T>(id, out var data))
		    {
			    callback?.Invoke(data);
		    }
		    else
		    {
			    /*var request = new FindOneMasterDataRequest<T>()
			    {
				    Id = id
			    };
			    request.OverrideStatus(StatusCode.MASTER_MODEL_NOT_FOUND, _ => { callback?.Invoke(default); });
			    request.OverrideStatus(StatusCode.MASTER_ID_INVALID, _ => { callback?.Invoke(default); });
			    request.Request<FindOneMasterDataResponse>(response =>
			    {
				    if (TryGetDescriptor(response.Data, out var descriptor))
				    {
					    var dataProtobuf = descriptor.Parser.ParseFrom(response.Data.Value);

					    Add(dataProtobuf);
					    callback?.Invoke(dataProtobuf as T);
				    }
				    else
				    {
					    callback?.Invoke(default);
				    }
			    });*/
		    }
	    }

	    public List<T> GetAll<T>() where T : class, IMessage, new()
	    {
	        var type = typeof(T);
	        if (MDataCache.ContainsKey(type))
	        {
	            var list = new List<T>();
	            foreach (var data in MDataCache[type])
	            {
	                list.Add(data.Value as T);
	            }
	            return list;
	        }
	        return null;
	    }

		public List<T> GetList<T>(IList<long> ids) where T : class, IMessage, new()
		{
			var type = typeof(T);
			if (MDataCache.ContainsKey(type))
			{
				var list = new List<T>();
				foreach (var id in ids)
				{
					if (TryToGet<T>(id, out var data))
					{
						list.Add(data);
					}
				}
				return list;
			}
			return null;
		}

		public void GetList<T>(List<long> ids, Action<List<T>> callback) where T : class, IMessage, new()
		{
			var result = new List<T>();
			var needDownloadDataIds = new List<long>();
			var needDownloadDataIndex = new List<int>();

			foreach (var id in ids)
			{
				if (id == 0) continue;

				if (TryGetFromCache<T>(id, out var data))
				{
					result.Add(data);
				}
				else
				{
					var index = result.Count;

					result.Add(default);
					needDownloadDataIds.Add(id);
					needDownloadDataIndex.Add(index);
				}
			}

			if (needDownloadDataIds.Count != 0)
			{
				/*var request = new FindManyMasterDataRequest<T>()
				{
					Ids = needDownloadDataIds
				};
				request.OverrideStatus(StatusCode.MASTER_MODEL_NOT_FOUND, _ => { callback?.Invoke(result); });
				request.OverrideStatus(StatusCode.MASTER_ID_INVALID, _ => { callback?.Invoke(result); });
				request.Request<FindManyMasterDataResponse>(response =>
				{
					foreach (var data in response.Data)
					{
						if (!TryGetDescriptor(data, out var descriptor)) continue;
						
						var dataProtobuf = descriptor.Parser.ParseFrom(data.Value);
						var id = (long)descriptor.FindFieldByName("id").Accessor.GetValue(dataProtobuf);
						if (!TryFindIndex(id, out var index)) continue;
						
						result[index] = dataProtobuf as T;
						Add(dataProtobuf);
					}
			
					callback?.Invoke(result);
				});*/
			}
			else
			{
				callback?.Invoke(result);
			}

			bool TryFindIndex(long id, out int indexResult)
			{
				for (var i = 0; i < needDownloadDataIds.Count; i++)
				{
					if (needDownloadDataIds[i] == id)
					{
						indexResult = needDownloadDataIndex[i];
						return true;
					}
				}

				indexResult = -1;
				return false;
			}
		}
		#endregion
		
		#endregion
	}
}