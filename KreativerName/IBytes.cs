namespace KreativerName
{
    /// <summary>
    /// An interface for byte management.
    /// </summary>
    public interface IBytes
    {
        /// <summary>
        /// Returns the object as bytes.
        /// </summary>
        /// <returns>The object as bytes.</returns>
        byte[] ToBytes();

        /// <summary>
        /// Creates an object from bytes.
        /// </summary>
        /// <param name="bytes">The byte array</param>
        /// <param name="startIndex">The start index in the array</param>
        /// <returns>The amount of bytes used</returns>
        int FromBytes(byte[] bytes, int startIndex);
    }
}
