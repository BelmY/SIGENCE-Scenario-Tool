using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using GeoAPI.Geometries;

using GMap.NET;

using NetTopologySuite.Geometries;
using NetTopologySuite.IO;



namespace SIGENCEScenarioTool.Tools
{
    /// <summary>
    /// 
    /// </summary>
    static public class GeoHelper
    {
        /// <summary>
        /// 
        /// </summary>
        static private readonly WKBReader wkbreader = new WKBReader();

        /// <summary>
        /// 
        /// </summary>
        static private readonly WKBWriter wkbwriter = new WKBWriter();

        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------


        /// <summary>
        /// 
        /// </summary>
        static public readonly Point GERMANY_CENTERPOINT = new Point( 51.133333333333 , 10.416666666667 );

        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------


        /// <summary>
        /// 
        /// </summary>
        /// <param name="strWKBAsString"></param>
        /// <returns></returns>
        static public IGeometry StringToGeometry( string strWKBAsString )
        {
            if( strWKBAsString == null || strWKBAsString.Trim().Length == 0 )
            {
                return null;
            }

            int iCount = strWKBAsString.Length >> 1;

            byte [] baData = new byte [iCount];

            using( StringReader sr = new StringReader( strWKBAsString ) )
            {
                for( int i = 0 ; i < iCount ; i++ )
                {
                    baData [i] = Convert.ToByte( new string( new char [2] { ( char ) sr.Read() , ( char ) sr.Read() } ) , 16 );
                }
            }

            return wkbreader.Read( baData );
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="geo"></param>
        /// <returns></returns>
        static public string GeometryToString( IGeometry geo )
        {
            // Kann ja passieren, ist ja auch nicht weiter schlimm, liefern wir halt eben auch null zurück.
            if( geo == null )
            {
                return null;
            }

            byte [] baData = wkbwriter.Write( geo );

            StringBuilder sb = new StringBuilder( baData.Length << 1 );

            foreach( byte b in baData )
            {
                sb.AppendFormat( "{0:X2}" , b );
            }

            return sb.ToString();
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------


        /// <summary>
        ///
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        static public Polygon CreatePolygon( params Point [] points )
        {
            if( points == null || points.Length < 3 )
            {
                throw new ArgumentException( "Es müssen mindestens drei Punkte übergeben werden!" );
            }

            List<Coordinate> lPoints = new List<Coordinate>( points.Length + 1 );

            // Die Koordinaten der Punkte können in die Liste kopieren.
            lPoints.AddRange( points.Select( p => p.Coordinate ) );

            // Damit es aber auch wirklich ein Polygon und nicht nur eine Line ist müssen wir den ersten Punkten noch hinten drann hängen damit es auch geschlossen wird.
            lPoints.Add( points [0].Coordinate );

            // Aus dem LinearRing können wir das Polygon erzeugen.
            return new Polygon( new LinearRing( lPoints.ToArray() ) );
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        static public PointLatLng CoordinateToPointLatLng( Coordinate c )
        {
            return new PointLatLng( c.Y , c.X );
        }

    } // end static public class GeoHelper
}