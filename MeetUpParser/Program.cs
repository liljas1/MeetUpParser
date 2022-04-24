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
            string output = CountryFormatter(locationsDict).TrimEnd().TrimEnd('|').TrimEnd();

            return output;
		}

        public static string CountryFormatter(Dictionary<string, List<string>> dictionary)
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

                location += CityAndStateFormatter(kvp.Value).TrimEnd();

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

        public static string CityAndStateFormatter(List<string> list)
		{
            //TODO: Change variable stateCity name
            string cityStateString = "";

            if(list.Count() <= 2)
			{
                for (int i = 0; i < list.Count(); i++)
                {
                    cityStateString += list[i];
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

                    cityStateString += list[i] + delimiter;
                }

                cityStateString = cityStateString.TrimEnd().TrimEnd('|');
            }

            return cityStateString;
		}
    }
}
