using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KreativerName
{
    public interface IBytes
    {
        /// <summary>
        /// Gibt das Objekt als Bytes zurück
        /// </summary>
        /// <returns>Das Objekt als Bytes</returns>
        byte[] ToBytes();

        /// <summary>
        /// Erstellt ein Objekt von Bytes und gibt die Anzahl an Bytes zurück, die benötigt wurden
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="startIndex"></param>
        /// <returns>Anzahl an Bytes, die benötigt wurden, um das Objekt zu erstellen</returns>
        int FromBytes(byte[] bytes, int startIndex);
    }
}
