package cn.pku.gis;

import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.net.URL;
import java.net.URLConnection;
import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.Statement;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

public class BDPOI
{

	public BDPOI()
	{
		// TODO Auto-generated constructor stub
	}

	public static void main(String[] args)
	{
		// TODO Auto-generated method stub
		fetchPOI("银行");  //keywords "bank", "银行" in Chinese
	}

	//get Beijing's POI data related to keywords from Baidu Map and store them in MySQL database.
	public static void fetchPOI(String keyword)
	{
		try
		{
			int idPOI = 0;  // POI id in MySQL database
			Class.forName("com.mysql.jdbc.Driver");
			String url = "jdbc:mysql://localhost:3306/poi_beijing?"
					+ "user=root&password=1234&useUnicode=true&characterEncoding=UTF8";
			Connection conn = DriverManager.getConnection(url);
			Statement stmt = conn.createStatement();
			int i=0;
			int j=0;
			//because of limited connection times, set the max connection times, for example 550*550
			for (i = 0; i < 550; i++)//550
			{
				for (j = 0; j < 550; j++)//550
				{
					int kPage = 0; //an query parameter indicating which page in your query results.
					while (true)
					{
						//get POIs related to the keyword in a rectangular region. The boundary of this rectangular is shown in the parameter.
						//The origin of the coordinate is (lng=115.3,lat= 39). The result return the kPage-th result.
						String result = sendGet(
								"http://api.map.baidu.com/place/v2/search",
								"ak=geTZ9FuBYqANGzG0dY1obYWf&output=json&page_size=20&scope=1&query="
										+ keyword
										+ "&bounds="+(39+j*0.0042)+","+(115.3+i*0.0042)+","+(39.0042+j*0.0042)+","+(115.3042+i*0.0042)
										+ "&page_num=" + kPage);
						//System.out.println(i+" "+j+" "+result);
						//parse the json and store POIs into database.
						try
						{
							JSONObject res = new JSONObject(result);
							if (res.getInt("status")==0&&res.getInt("total") != 0)
							{
								JSONArray locs = res.getJSONArray("results");
								for (int ii = 0; ii < locs.length(); ii++)
								{
									JSONObject poi = (JSONObject) locs.get(ii);
									String name = poi.getString("name").replaceAll("\"", "");
									Double lat = poi.getJSONObject("location")
											.getDouble("lat");
									Double lng = poi.getJSONObject("location")
											.getDouble("lng");
									String address = poi.getString("address").replaceAll("\"", "");
									//String street_id = poi.getString("street_id");
									//String telephone = poi.getString("telephone");
									int detail = poi.getInt("detail");
									String uid = poi.getString("uid");

									String sql = "INSERT INTO poi_beijing.poi VALUES("
											+ idPOI + ",\"" + name + "\"," + lat
											+ "," + lng + ",\"" + address + "\"," 
											+ detail + ",\"" + uid + "\");";
									stmt.executeUpdate(sql);
									idPOI++; //increase POI id by 1
								}
								kPage++; //increase the page id by 1.
							}
							else
								break;
						}
						catch (JSONException e)
						{
							// TODO Auto-generated catch block
							e.printStackTrace();
						}
					}
				}
			}
			//System.out.println(i+","+j);
		}
		catch (Exception e1)
		{
			// TODO Auto-generated catch block
			e1.printStackTrace();
		}

	}

	//connect Baidu Map API and get the data
	public static String sendGet(String url, String param)
	{
		String result = "";
		BufferedReader in = null;
		try
		{
			String urlNameString = url + "?" + param;
			URL realUrl = new URL(urlNameString);
			URLConnection connection = realUrl.openConnection();
			connection.setRequestProperty("accept", "*/*");
			connection.setRequestProperty("connection", "Keep-Alive");
			connection.setRequestProperty("user-agent",
					"Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1;SV1)");
			connection.connect();
			in = new BufferedReader(
					new InputStreamReader(connection.getInputStream()));
			String line;
			while ((line = in.readLine()) != null)
			{
				result += line;
			}
		}
		catch (Exception e)
		{
			System.out.println("Fail to sent get request: " + e);
			e.printStackTrace();
		}
		finally
		{
			try
			{
				if (in != null)
				{
					in.close();
				}
			}
			catch (Exception e2)
			{
				e2.printStackTrace();
			}
		}
		return result;
	}
}
