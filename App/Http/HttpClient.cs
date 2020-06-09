using System;
using System.IO;
using System.Net;
using System.Text;
using Core;
using Newtonsoft.Json;
using UpdateService;
using UpdateService.Common;

namespace Medical.AppLayer.Http
{
    public class HttpClient
    {
        private string _jsonString;
        public UpdateStream GetVersion(int version)
        {
            string url = Constants.ServiceUrl() + "getupdate/version";

            WebRequest request = WebRequest.Create(url);
            request.Timeout = 5000;
            request.Method = "POST";
            request.ContentType = "application/json";

            if (_jsonString == String.Empty)
                throw new Exception("Во входящем запросе не найден ожидаемый тег");

            byte[] byteArray = Encoding.UTF8.GetBytes(version.ToString());
            request.ContentLength = byteArray.Length;


            var listMo = new UpdateStream();
            try
            {
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                //Возвращает ответ на запрос
                WebResponse response = request.GetResponse();

                if (((HttpWebResponse)response).StatusCode == HttpStatusCode.OK)
                {
                    dataStream = response.GetResponseStream();
                    if (dataStream != null)
                    {
                        StreamReader reader = new StreamReader(dataStream);
                        string responseFromServer = reader.ReadToEnd();
                        listMo = JsonConvert.DeserializeObject<UpdateStream>(responseFromServer);
                        // Console.WriteLine(responseFromServer);
                        reader.Close();
                        dataStream.Close();
                        return listMo;
                    }
                }
                else
                {
                    listMo.Data = null;
                    listMo.Status = 1;
                    listMo.Message = $"Опа! Что-то пошло не так.";
                }
                response.Close();
            }
            catch (Exception ex)
            {
                listMo.Data = null;
                listMo.Status = 1;
                listMo.Message = $"Опа! Что-то пошло не так. {ex.Message}";
            }

            return listMo;
        }

        public UpdateStream GetVersionJson(int version, TypeProductUpdate type)
        {
            string url = Constants.ServiceUrl() + "getupdate/version";
            //"http://192.168.10.149:8090/updateservice/getupdate/version"; //Constants.ServiceUrl() + "getupdate/version";
            string json = "{\"version\":\"version_\",\"type\": \"type_\"}";

            WebRequest request = WebRequest.Create(url);
            request.Timeout = 10000;
            request.Method = "POST";
            request.ContentType = "application/json";

            json = json.Replace("version_", version.ToString()).Replace("type_", type.ToString());

            byte[] byteArray = Encoding.UTF8.GetBytes(json);
            request.ContentLength = byteArray.Length;


            var listMo = new UpdateStream();
            try
            {
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                //Возвращает ответ на запрос
                WebResponse response = request.GetResponse();

                if (((HttpWebResponse) response).StatusCode == HttpStatusCode.OK)
                {
                    dataStream = response.GetResponseStream();
                    if (dataStream != null)
                    {
                        StreamReader reader = new StreamReader(dataStream);
                        string responseFromServer = reader.ReadToEnd();
                        listMo = JsonConvert.DeserializeObject<UpdateStream>(responseFromServer);
                        // Console.WriteLine(responseFromServer);
                        reader.Close();
                        dataStream.Close();
                        return listMo;
                    }
                }
                else
                {
                    listMo.Data = null;
                    listMo.Status = 1;
                    listMo.Message = $"Опа! Что-то пошло не так.";
                }

                response.Close();
            }
            catch (Exception ex)
            {
                listMo.Data = null;
                listMo.Status = 1;
                listMo.Message = $"Опа! Что-то пошло не так. {ex.Message}";
            }

            return listMo;

        }

        public WrapperAnswer<string> GetConnect()
        {
            string url = Constants.ServiceUrl() + "connect";
            var listMo = new WrapperAnswer<string>();
            string responseFromServer = string.Empty;
            try
            {
                WebRequest req = WebRequest.Create(url);
                WebResponse resp = req.GetResponse();

                if (((HttpWebResponse)resp).StatusCode == HttpStatusCode.OK)
                {

                    Stream stream = resp.GetResponseStream();
                    StreamReader sr = new StreamReader(stream);
                    responseFromServer = sr.ReadToEnd();
                    sr.Close();
                    listMo.Data = responseFromServer;
                    return listMo;
                }
                else
                {
                    listMo.AddError("Ошибка соединения");
                }
            }
            catch (Exception ex)
            {
                listMo.AddError(ex.Message);
            }

            return listMo;
        }
    }
}
