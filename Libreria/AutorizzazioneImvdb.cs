using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

using ICSharpCode.SharpZipLib.GZip;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
//using Windows.Web.Http;   //uso questa classe in UWP
using System.Threading;     //questo mi serve per la versione Android


using System.Net.Http;       //uso questa classe per applicazioni multipiattaforma UWP e Xamarin
using System.Net.Http.Headers;
namespace Imvdb.LibreriaImvdb
{


    class AutorizzazioneImvdb
    {
        public string application_id = "";
        public string application_key = "";
    }

}