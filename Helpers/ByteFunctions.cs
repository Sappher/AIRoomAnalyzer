namespace Helpers
{
    public class ByteFuntions
    {
        public static bool IsPngImage(Stream stream)
        {
            // Save the current position
            long originalPosition = stream.Position;

            // Check if the data starts with PNG signature
            byte[] pngSignature = new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }; // PNG signature
            byte[] buffer = new byte[pngSignature.Length];

            stream.Position = 0; // Set the position to the beginning of the stream
            int bytesRead = stream.Read(buffer, 0, buffer.Length);

            // Restore the original position
            stream.Position = originalPosition;

            return bytesRead == pngSignature.Length && CompareBytes(buffer, pngSignature);
        }

        public static bool CompareBytes(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;

            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                    return false;
            }
            return true;
        }

        public static bool IsPngEndChunk(Stream stream)
        {
            // PNG End chunk type
            byte[] iendChunkType = new byte[] { 73, 69, 78, 68 }; // IEND

            // Save the current position
            long originalPosition = stream.Position;

            // Move to the end of the stream and then move back to check for IEND
            stream.Seek(-12, SeekOrigin.End);

            // Read the chunk length
            byte[] lengthBytes = new byte[4];
            stream.Read(lengthBytes, 0, 4);

            // Read the chunk type
            byte[] chunkTypeBytes = new byte[4];
            stream.Read(chunkTypeBytes, 0, 4);

            // Restore the original position
            stream.Position = originalPosition;

            // Check if it's the IEND chunk
            return CompareBytes(chunkTypeBytes, iendChunkType);
        }
    }

}