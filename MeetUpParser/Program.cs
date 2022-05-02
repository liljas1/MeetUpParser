using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.Linq;

namespace MeetUpParser
{
	public class Program
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
            dynamic inputJson = JsonConvert.DeserializeObject(json);
            dynamic meetings = inputJson["input"];

            List<string> outputString = new List<string>();

            foreach (var meeting in meetings)
            {
                string meetingAndDate = EditionNameDateFormatter(meeting);
                string locations = LocationsFormatter(meeting["location"]);

                meetingAndDate += locations;

                outputString.Add(meetingAndDate.TrimStart());
            }

            IDictionary<string, List<string>> parsedMeetings = new Dictionary<string, List<string>>();
            parsedMeetings.Add("meetUps", outputString);

            dynamic outputJson = JsonConvert.SerializeObject(parsedMeetings);

            return outputJson;
		}

        public static string EditionNameDateFormatter(dynamic meeting)
		{
            string line = $"{meeting["edition"] ?? ""} {meeting["name"] ?? ""} · {meeting["startDate"] ?? ""} ";

            if (meeting["endDate"] != null)
            {
                line += $"/ {meeting["endDate"]} · ";
            }
            else
            {
                line += "· ";
            }

            return line;
		}

        public static string LocationsFormatter(dynamic locations)
		{
            Dictionary<string, List<string>> locationsDict = new Dictionary<string, List<string>>();

            foreach (dynamic location in locations)
			{
                string state = "";
                string city = "";
                string country = location["country"].ToString();
                string delimiter = "";

                if (location["state"] != null)
                {                    
                    if(location["city"] != null)
					{
                        delimiter = ", ";
					}
                    state = delimiter + location["state"].ToString();
                }

                if (location["city"] != null)
                {
                    city = location["city"].ToString();
                }

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

            return CountryFormatter(locationsDict);
		}

        public static string CountryFormatter(Dictionary<string, List<string>> dictionary)
		{
            string formattedString = "";
            string delimiter = "";
            string sufix = "";

            foreach (KeyValuePair<string, List<string>> location in dictionary)
            {
                int stateCount = 0;
                int cityCount = 0;
                string country = location.Key;
                List<string> cityStateList = location.Value;

                formattedString += CityAndStateFormatter(cityStateList).TrimEnd();

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
                formattedString += country;
            }

            return formattedString.TrimEnd().TrimEnd('|').TrimEnd();
        }

        public static string CityAndStateFormatter(List<string> cityStateList)
		{
            string cityStateString = "";

            if(cityStateList.Count() <= 2)
			{
                for (int i = 0; i < cityStateList.Count(); i++)
                {
                    cityStateString += cityStateList[i];
                }
            }
			else
			{
                for (int i = 0; i < cityStateList.Count(); i++)
                {
                    string delimiter = "";

                    if (i % 2 != 0 && cityStateList[i-1] != "")
                    {
                        delimiter = " | ";

                    }

                    cityStateString += cityStateList[i] + delimiter;
                }

                cityStateString = cityStateString.TrimEnd().TrimEnd('|');
            }

            return cityStateString;
		}
    }
}
