using UnityEditor;
namespace XGame
{
	public class ChangeLanguage4Editor
	{
		static void ResetLanguage(string languageName)
		{
			EditorPrefs.SetString("language", languageName);
			LanguageManager.Instance.Init(languageName.ToEnumT<LanguageTypeEnum>());
		}
		[MenuItem("Languages/English", false, 8)]
		static void ToEnglish()
		{
			ResetLanguage("English");
		}
		[MenuItem("Languages/English", true)]
		static bool CheckEnglishUsed()
		{
			return EditorPrefs.GetString("language")!= "English";
		}
		[MenuItem("Languages/Japanese", false, 8)]
		static void ToJapanese()
		{
			ResetLanguage("Japanese");
		}
		[MenuItem("Languages/Japanese", true)]
		static bool CheckJapaneseUsed()
		{
			return EditorPrefs.GetString("language")!= "Japanese";
		}
		[MenuItem("Languages/Korean", false, 8)]
		static void ToKorean()
		{
			ResetLanguage("Korean");
		}
		[MenuItem("Languages/Korean", true)]
		static bool CheckKoreanUsed()
		{
			return EditorPrefs.GetString("language")!= "Korean";
		}
		[MenuItem("Languages/Hindi", false, 8)]
		static void ToHindi()
		{
			ResetLanguage("Hindi");
		}
		[MenuItem("Languages/Hindi", true)]
		static bool CheckHindiUsed()
		{
			return EditorPrefs.GetString("language")!= "Hindi";
		}
		[MenuItem("Languages/Russian", false, 8)]
		static void ToRussian()
		{
			ResetLanguage("Russian");
		}
		[MenuItem("Languages/Russian", true)]
		static bool CheckRussianUsed()
		{
			return EditorPrefs.GetString("language")!= "Russian";
		}
		[MenuItem("Languages/Portuguese", false, 8)]
		static void ToPortuguese()
		{
			ResetLanguage("Portuguese");
		}
		[MenuItem("Languages/Portuguese", true)]
		static bool CheckPortugueseUsed()
		{
			return EditorPrefs.GetString("language")!= "Portuguese";
		}
		[MenuItem("Languages/German", false, 8)]
		static void ToGerman()
		{
			ResetLanguage("German");
		}
		[MenuItem("Languages/German", true)]
		static bool CheckGermanUsed()
		{
			return EditorPrefs.GetString("language")!= "German";
		}
		[MenuItem("Languages/French", false, 8)]
		static void ToFrench()
		{
			ResetLanguage("French");
		}
		[MenuItem("Languages/French", true)]
		static bool CheckFrenchUsed()
		{
			return EditorPrefs.GetString("language")!= "French";
		}
		[MenuItem("Languages/ChineseTraditional", false, 8)]
		static void ToChineseTraditional()
		{
			ResetLanguage("ChineseTraditional");
		}
		[MenuItem("Languages/ChineseTraditional", true)]
		static bool CheckChineseTraditionalUsed()
		{
			return EditorPrefs.GetString("language")!= "ChineseTraditional";
		}
		[MenuItem("Languages/Indonesian", false, 8)]
		static void ToIndonesian()
		{
			ResetLanguage("Indonesian");
		}
		[MenuItem("Languages/Indonesian", true)]
		static bool CheckIndonesianUsed()
		{
			return EditorPrefs.GetString("language")!= "Indonesian";
		}
		[MenuItem("Languages/Spanish", false, 8)]
		static void ToSpanish()
		{
			ResetLanguage("Spanish");
		}
		[MenuItem("Languages/Spanish", true)]
		static bool CheckSpanishUsed()
		{
			return EditorPrefs.GetString("language")!= "Spanish";
		}
		[MenuItem("Languages/Pilipinas", false, 8)]
		static void ToPilipinas()
		{
			ResetLanguage("Pilipinas");
		}
		[MenuItem("Languages/Pilipinas", true)]
		static bool CheckPilipinasUsed()
		{
			return EditorPrefs.GetString("language")!= "Pilipinas";
		}
		[MenuItem("Languages/Thai", false, 8)]
		static void ToThai()
		{
			ResetLanguage("Thai");
		}
		[MenuItem("Languages/Thai", true)]
		static bool CheckThaiUsed()
		{
			return EditorPrefs.GetString("language")!= "Thai";
		}
		[MenuItem("Languages/Laothian", false, 8)]
		static void ToLaothian()
		{
			ResetLanguage("Laothian");
		}
		[MenuItem("Languages/Laothian", true)]
		static bool CheckLaothianUsed()
		{
			return EditorPrefs.GetString("language")!= "Laothian";
		}
		[MenuItem("Languages/Vietnamese", false, 8)]
		static void ToVietnamese()
		{
			ResetLanguage("Vietnamese");
		}
		[MenuItem("Languages/Vietnamese", true)]
		static bool CheckVietnameseUsed()
		{
			return EditorPrefs.GetString("language")!= "Vietnamese";
		}
	}
}