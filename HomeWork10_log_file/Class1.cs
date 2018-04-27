using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MyLoger
{
	/*Реализовать программный модуль (класс Loger), 
	 * позволяющий записывать информацию о работе программы в log-файл.
	 *  Каждая запись информации в log-файл должна содержать следующие поля: 
	 *   дата и время, тип сообщения (ошибка, exception, тестовое сообщение,
	 *    информационное сообщение, предупреждение), имя текущего пользователя, текст сообщения. 
	 *     При этом должна быть реализована настройка данного класса.
	 *      В данной настройке указывается маска записи запись информации в log-файл
	 *       т.е. порядок информации в записи и наличие указанных полей полей. 
	 *        Настройка считывается из файла конфигурации (ini-файла). */
    public static class Loger
    {
		static Dictionary<string, int> dictionary;
		static Loger()
		{
			string iniFile = "setup.ini";
			if (!File.Exists(iniFile))
				throw new FileNotFoundException();

			dictionary = new Dictionary<string, int>();
			using (StreamReader sr = new StreamReader(iniFile, Encoding.GetEncoding(1251)))
			{
				while (!sr.EndOfStream)
				{
					string[] tmp = new string[2] { sr.ReadLine(), "" };
					if (tmp[0].Length < 2 || tmp[0][0] == '#' || tmp[0][0] == '/')
						continue;

					tmp = tmp[0].Split(new char[] { ' ', '=' }, StringSplitOptions.RemoveEmptyEntries);
					if (tmp.Length <= 1)
						continue;
					dictionary[tmp[0]] = Convert.ToInt32(tmp[1]);
				}
			}
		}
		public enum MsgType
		{
			ERROR,
			TEXT,
			INFO,
			WARNING
		}
		public static bool Log(string FilePath, MsgType type, string message)
		{
			string formatted = "L ";
			bool time, msgtype, name, text;
			time = msgtype = name = text = false;
			foreach (KeyValuePair<string, int> item in dictionary)
			{
				if (item.Value != 1)
					continue;
				if (item.Key == "Time" && !time)
					formatted += DateTime.Now + " ";
				else if (item.Key == "MsgType" && !msgtype)
					formatted += $"[{type.ToString()}] ";
				else if (item.Key == "UserName" && !name)
					formatted += $"{Environment.UserName} ";
				else if (item.Key == "Text" && !text)
					formatted += $": {message}";
			}
			if (formatted.Length < 5)
				return false;

			using (FileStream fs = new FileStream("LogFile.log", FileMode.Append))
			{
				using (StreamWriter sr = new StreamWriter(fs, Encoding.Unicode))
				{
					sr.WriteLine(formatted);
				}
			}
			return true;
		}
		public static bool Log(MsgType type, string message)
		{
			return Log($"{DateTime.Now.Year}_{DateTime.Now.Month}_{DateTime.Now.Day}.log", type, message);
		}
    }
}
