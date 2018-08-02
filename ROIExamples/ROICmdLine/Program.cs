using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Net.Http;
using System.Net.Http.Headers;


namespace ROICmdLine
{
    class Program
    {
        // default address of MAPPS ROI interface.
        private const string uri = "http://127.0.0.1:9545";

        // http://www.eyesdx.com/client/userguide/general/edx_roi_rest_api.html
        static void Main(string[] args) { 
        
            // setup the client URI
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(uri); 
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

            // Start w/ a clear ROI list.
            //  DELETE: /roi    This call deletes all ROIs in the current MAPPS dataset.
            string command = "/roi";
            HttpResponseMessage response = client.DeleteAsync(command).Result;
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("DELETE all returned successfully");
            }
            else
            {
                Console.WriteLine("DELETE error:  {0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }


            // create a static rect ROI
            //  PUT: /roi/{roiName}
            //   roiName - Name of the ROI to be created.  The name must be unique.  Duplicate names will overwrite the previous ROI
            //   shape (required) — Shape of the roi ('rectangular', 'ellipse', 'points').
            //   x(required)  — Decimal value (0.0-1.0) for 'x' for the ROI. For a rectangular ROI, this is the lower left corner. 
            //   y(required)  — Decimal value (0.0-1.0) for 'y' for the ROI. For a rectangular ROI, this is the lower left corner. 
            //   w(required)  — Decimal value (0.0-1.0) for the 'w' for the ROI. For a rectangular ROI, this is the full width
            //                  along the x‐axis.
            //   h(required)  — Decimal value (0.0-1.0) for the 'h' for the ROI. For a rectangular ROI, this is the full height
            //                  along the y‐axis.
            //   screen_index (optional) — Integer value (0,1,2,3,4,5,6,7,8,9) indicates which screen recieves the ROI.  Defaults to screen 0
            command = "/roi/static_rect?shape=rectangular&x=0.25&y=0.25&w=0.5&h=0.5&screen_index=0";
            HttpContent content = null;
            response = client.PutAsync(command, content).Result;
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("PUT roi/static_rect returned successfully");
            }
            else
            {
                Console.WriteLine("PUT error:  {0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }


            // create a dynamic Rect ROI with a frame 60 seconds in the future and 0 seconds into the future.
            //  PUT: /roi/{roiName}/frame
            //   roiName - Name of the ROI to be created.  The name must be unique.  Duplicate names will overwrite the previous ROI
            //   shape (required) — Shape of the roi ('rectangular', 'ellipse', 'points').
            //   x(required)  — Decimal value (0.0-1.0) for 'x' for the ROI. For a rectangular ROI, this is the lower left corner. 
            //   y(required)  — Decimal value (0.0-1.0) for 'y' for the ROI. For a rectangular ROI, this is the lower left corner. 
            //   w(required)  — Decimal value (0.0-1.0) for the 'w' for the ROI. For a rectangular ROI, this is the full width
            //                  along the x‐axis.
            //   h(required)  — Decimal value (0.0-1.0) for the 'h' for the ROI. For a rectangular ROI, this is the full height
            //                  along the y‐axis.
            //   time (optional) — UTC Time in windows Filetime format (the number of 100-nanosecond intervals since January 1, 1601).  0 uses current time
            //   screen_index (optional) — Integer value (0,1,2,3,4,5,6,7,8,9) indicates which screen receives the ROI.  Defaults to screen 0
            var now = DateTime.Now.ToFileTimeUtc();           // current UTC time 
            var time = now + Convert.ToInt64(60 * 10000000);  // delay by 60 seconds
            command = "/roi/dynamic_rect/frame?shape=rectangular&x=0.3&y=0.3&w=0.3&h=0.3&time=" + time;
            content = null;
            response = client.PutAsync(command, content).Result;
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("PUT roi/dynamic_rect returned successfully");
                var responseContent = response.Content;
                string responseString = responseContent.ReadAsStringAsync().Result;
                Console.WriteLine(responseString);
            }
            else
            {
                Console.WriteLine("PUT error:  {0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }
            // delay by 0 seconds
            time = now;
            command = "/roi/dynamic_rect/frame?shape=rectangular&x=0.5&y=0.5&w=0.45&h=0.45&time=" + time;
            content = null;
            response = client.PutAsync(command, content).Result;
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("PUT roi/dynamic_rect returned successfully");
                var responseContent = response.Content;
                string responseString = responseContent.ReadAsStringAsync().Result;
                Console.WriteLine(responseString);
            }
            else
            {
                Console.WriteLine("PUT error:  {0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }

            // delay by 30 seconds
            time = now + Convert.ToInt64(30 * 10000000); ;
            command = "/roi/dynamic_rect/frame?shape=rectangular&x=0.5&y=0.5&w=0.1&h=0.1&time=" + time;
            content = null;
            response = client.PutAsync(command, content).Result;
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("PUT roi/dynamic_rect returned successfully");
                var responseContent = response.Content;
                string responseString = responseContent.ReadAsStringAsync().Result;
                Console.WriteLine(responseString);
            }
            else
            {
                Console.WriteLine("PUT error:  {0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }



            // create a static ellipse ROI
            //  PUT: /roi/{roiName}
            //   roiName - Name of the ROI to be created.  The name must be unique.  Duplicate names will overwrite the previous ROI
            //   shape (required) — Shape of the roi.  Set to 'ellipse'
            //   x(required)  — Decimal value (0.0-1.0) for 'x' for the ROI.  For an elliptical ROI, this is the center of the ellipse.
            //   y(required)  — Decimal value (0.0-1.0) for 'y' for the ROI.  For an elliptical ROI, this is the center of the ellipse.
            //   w(required)  — Decimal value (0.0-1.0) for the 'w' for the ROI. For an elliptical ROI, this is the radius of the ellipse along the x‐axis.
            //   h(required)  — Decimal value (0.0-1.0) for the 'h' for the ROI. For an elliptical ROI, this is the radius of the ellipse along the y‐axis.
            //   screen_index (optional) — Integer value (0,1,2,3,4,5,6,7,8,9) indicates which screen recieves the ROI.  Defaults to screen 0
            command = "/roi/static_ellipse?shape=ellipse&x=0.75&y=0.755&w=0.2&h=0.2&screen_index=0";
            content = null;
            response = client.PutAsync(command, content).Result;
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("PUT roi/static_ellipse returned successfully");
            }
            else
            {
                Console.WriteLine("PUT error:  {0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }


            // create a static shape ROI
            //  PUT: /roi/{roiName}
            //   roiName - Name of the ROI to be created.  The name must be unique.  Duplicate names will overwrite the previous ROI
            //   shape (required) — Shape of the roi.  Set to 'points' for polygonal shape
            //   points(required) — Point array for free‐form ROI.Points are formatted as 'x1 y1;x2 y2;x3 y3;x4 y4;...'
            command = "/roi/static_shape?shape=points&points=0.1,0.1;0.5,0.95;0.9,0.25;0.5,0.5;";
            content = null;
            response = client.PutAsync(command, content).Result;
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("PUT roi/static_shape returned successfully");
            }
            else
            {
                Console.WriteLine("PUT error:  {0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }



            // create a dynamic ellipse ROI  
            time = now + Convert.ToInt64(10 * 10000000);  // delay by 10 seconds
            command = "/roi/dynamic_ellipse/frame?shape=ellipse&x=0.0&y=0.0&w=0.3&h=0.3&time=" + time;
            content = null;
            response = client.PutAsync(command, content).Result;
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("PUT roi/dynamic_ellipse returned successfully");
                var responseContent = response.Content;
                string responseString = responseContent.ReadAsStringAsync().Result;
                Console.WriteLine(responseString);
            }
            else
            {
                Console.WriteLine("PUT error:  {0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }
            // delay by 10 seconds
            time = now - Convert.ToInt64(10 * 10000000); // jump back 10 seconds PRIOR to now.
            command = "/roi/dynamic_ellipse/frame?shape=ellipse&x=1.0&y=1.0&w=0.3&h=0.3&time=" + time;
            content = null;
            response = client.PutAsync(command, content).Result;
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("PUT roi/dynamic_ellipse returned successfully");
                var responseContent = response.Content;
                string responseString = responseContent.ReadAsStringAsync().Result;
                Console.WriteLine(responseString);
            }
            else
            {
                Console.WriteLine("PUT error:  {0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }
 



            // get the whole list of ROIS
            //  GET: /roi    This call retrieves all of the ROI currently stored in MAPPS as an XML documen
            command = "/roi";
            response = client.GetAsync(command).Result;
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("GET all returned successfully");
                string data = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine(data);
            }
            else
            {
                Console.WriteLine("GET error:  {0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }


            // get a single ROI by name
            //  GET: /roi/{roiName}    This call retrieves an ROI based on its unique name
            command = "/roi/dynamic_rect";
            response = client.GetAsync(command).Result;
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("GET returned successfully");
                string data = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine(data);
            }
            else
            {
                Console.WriteLine("GET error:  {0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }


            // delete a single ROI by name 
            //  DELETE: /roi{roiName}    This call deletes an ROI based on its unique name
            command = "/roi/static_rect";
            response = client.DeleteAsync(command).Result;
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("DELETE returned successfully");
            }
            else
            {
                Console.WriteLine("DELETE error:  {0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }
           
            client.Dispose();
        }
    }
}
