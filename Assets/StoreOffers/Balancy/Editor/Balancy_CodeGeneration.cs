using System;
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;
using Balancy.Dictionaries;

namespace Balancy.Editor
{
	public static class Balancy_CodeGeneration
	{
#pragma warning disable 649
		[Serializable]
		private class GeneratedClass
		{
			public string className;
			public string classCode;
			public string relativePath;
		}

		[Serializable]
		private class GeneratedCode : ResponseData
		{
			public GeneratedClass[] list;
		}
#pragma warning restore 649

		private static Loader m_Loader;
		private static IEnumerator m_Coroutine;
		
		public static void StartGeneration(string gameId, string publicKey, Constants.Environment env, Action onComplete, string savePath)
		{
			m_Loader = new Loader(new AppConfig
			{
				ApiGameId = gameId,
				PublicKey = publicKey,
				Environment = env
			}, true);
			
			m_Coroutine = m_Loader.GetClasses(res =>
			{
				AssetDatabase.Refresh();
				
				var response = JsonUtility.FromJson<GeneratedCode>(res);
				if (response.Success)
				{
					ParseResponse(response, savePath);
					onComplete?.Invoke();
				} else
				{
					EditorUtility.DisplayDialog("Error", response.Error.Message, "Ok");
					onComplete();
				}
			});
			
			EditorCoroutineHelper.Execute(m_Coroutine);
		}

		private static void ParseResponse(GeneratedCode code, string savePath)
		{
			if (Directory.Exists(savePath))
				Directory.Delete(savePath, true);

			Directory.CreateDirectory(savePath);

			SaveFiles(code, savePath);

			AssetDatabase.Refresh();
		}

		private static void SaveFiles(GeneratedCode code, string savePath)
		{
			foreach (var cl in code.list)
			{
				var folderPath = savePath + cl.relativePath;
				if (!string.IsNullOrEmpty(cl.relativePath))
				{
					if (!Directory.Exists(folderPath))
						Directory.CreateDirectory(folderPath);
				}

				var path = $"{folderPath}/{cl.className}.cs";
				using (StreamWriter sw = File.CreateText(path))
				{
					sw.Write(cl.classCode);
					sw.Close();
				}
			}
		}
	}
}