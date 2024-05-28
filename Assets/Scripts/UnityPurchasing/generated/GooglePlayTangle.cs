// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("VuRnRFZrYG9M4C7gkWtnZ2djZmWPuziiIGVcfJCJ4o1c/ySJAssAH9f5jcfCKBzJ+umrLf5VhftLXIA9ZoD8y+N14MkLWrMi83j+Rq7j7a4znc9rUyEf/3spPiyZPEF7gE3hSEeXCPJPOqorO6QkO95LZPuUkeYa611RJud0mziHkKw//OU6+5NiE4zP5umENdL74LAyRpQQrtk8fX/KoxiXGqDPxdkfIkYkOmzGReji/obgU8V5ksf52D84cog8hlaypRUZvTADgqQfqiqoy44EqOCpW4JI1eYwCuRnaWZW5GdsZORnZ2biNcl8UzCxFegB4tNEEkNwW6eWZBa+jP0MEqVbLSIjpmFTQZPeKPVJz49wxulUsFG8mfJwHIR4T2RlZ2Zn");
        private static int[] order = new int[] { 0,5,12,6,9,13,8,10,8,12,12,13,13,13,14 };
        private static int key = 102;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
