using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.Linq;

namespace MeetUpParser
{
	class Program
	{
        static void Main(string[] args)
        {
            string json = @"{
   'input':[
      {
                'edition':'4th',
         'name':'JBCN Conference',
         'startDate':'2018-06-11',
         'endDate':'2018-06-13',
         'location':[
            {
                    'city':'Barcelona',
               'country':'Spain'
            }
         ]
      },
      {
                'edition':'3rd',
         'name':'DevTernity',
         'startDate':'2018-11-30',
         'endDate':'2018-12-01',
         'location':[
            {
                    'city':'Riga',
               'country':'Latvia'
            }
         ]
      },
      {
                'edition':'1st',
         'name':'I T.A.K.E Unconference',
         'startDate':'2016-05-19',
         'endDate':'2016-05-20',
         'location':[
            {
                    'city':'Bucharest',
               'country':'Romania'
            },
            {
                    'city':'Maramures',
               'country':'Romania'
            }
         ]
      },
      {
                'edition':'2nd',
         'name':'Product Owner Rule Book',
         'startDate':'2016-04-11',
         'endDate':'2016-04-13',
         'location':[
            {
                    'city':'Paris',
               'country':'France'
            },
            {
                    'city':'Madrid',
               'country':'Spain'
            }
         ]
      },
      {
                'name':'Upfront Summit',
         'startDate':'2018-02-01',
         'location':[
            {
                    'city':'Los Angeles',
               'state':'California',
               'country':'United States'
            }
         ]
      },
      {
                'name':'IBM Think',
         'startDate':'2018-03-19',
         'location':[
            {
                    'state':'Nevada',
               'country':'United States'
            }
         ]
      }
   ]
}";
            Console.WriteLine(MeetUpParser(json));
        }

        public static string MeetUpParser(string json)
		{
            dynamic input = JsonConvert.DeserializeObject(json);
            dynamic meetings = input.input;

            List<string> list = new List<string>();

            foreach (var meet in meetings)
            {
                string locations = "";

                //TODO: Rename variable
                string lines = $"{meet["edition"] ?? ""} {meet["name"] ?? ""} · {meet["startDate"] ?? ""} ";

                if (meet["endDate"] != null)
				{
                    lines += $"/ {meet["endDate"]} · ";
				}
				else
				{
                    lines += "· ";

                }

                locations = FindMatchingCities(meet["location"]);

                lines += locations;

                list.Add(lines.TrimStart());
            }

            IDictionary<string, List<string>> conferences = new Dictionary<string, List<string>>();
            conferences.Add("meetUps", list);

            dynamic output = JsonConvert.SerializeObject(conferences);
            return output;
		}

        public static string FindMatchingCities(dynamic locations)
		{
            Dictionary<string, List<string>> locationsDict = new Dictionary<string, List<string>>();

            foreach (dynamic location in locations)
			{
                string state = "";
                string city = "";
                string country = location["country"].ToString();
                string delimiter = "";

                //TODO: Rewrite
                if (location["state"] != null)
                {                    
                    if(location["city"] != null)
					{
                        delimiter = ", ";
					}
                    state = delimiter + location["state"].ToString();
                }

                //TODO: Rewrite
                if (location["city"] != null)
                {
                    city = location["city"].ToString();
                }

                //TODO: Extract to method (maybe)
                if (locationsDict.ContainsKey(country))
                {
                    List<string> locationList = locationsDict[country];
                    locationList.AddRange(new List<string> { city, state });
                }
				else
				{
                    locationsDict.Add(country, new List<string> { city, state });
				}
			}

            //TODO: rename output variable
            string output = PrintLocation(locationsDict).TrimEnd().TrimEnd('|').TrimEnd();

            return output;
		}

        //TODO: Change method name
        public static string PrintLocation(Dictionary<string, List<string>> dictionary)
		{
            string location = "";
            string delimiter = "";
            string sufix = "";

            foreach (KeyValuePair<string, List<string>> kvp in dictionary)
            {
                int stateCount = 0;
                int cityCount = 0;
                string country = kvp.Key;
                List<string> cityStateList = kvp.Value;

                location += PrintStatesAndCities(kvp.Value).TrimEnd();

                for (int i = 0; i < cityStateList.Count(); i++)
                {
                    if (i % 2 != 0 && cityStateList[i] != "") stateCount++;
                    if (i % 2 == 0 && cityStateList[i] != "") cityCount++;
                }

                if (country.Count() > 1)
				{
                    sufix = " | ";
				}

                if (stateCount > 0 && cityCount > 0)
				{
                    delimiter = ". ";
                }
                else
                {
                    delimiter = ", ";
                }

                country = delimiter + country + sufix;
                location += country;
            }

            return location;
        }

        //TODO: Change method name
        public static string PrintStatesAndCities(List<string> list)
		{
            //TODO: Change variable stateCity name
            string stateCity = "";

            if(list.Count() <= 2)
			{
                for (int i = 0; i < list.Count(); i++)
                {
                    stateCity += list[i];
                }
            }
			else
			{
                for (int i = 0; i < list.Count(); i++)
                {
                    string delimiter = "";

                    if (i % 2 != 0 && list[i-1] != "")
                    {
                        delimiter = " | ";

                    }

                    stateCity += list[i] + delimiter;
                }

                stateCity = stateCity.TrimEnd().TrimEnd('|');
            }

            return stateCity;
		}
    }
}
