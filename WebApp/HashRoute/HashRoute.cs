using System.Security.Cryptography;
using System.Text;

public class HashRoute : IOutboundParameterTransformer
{
    public string TransformOutbound(object value)
    {
        string input = value.ToString();
        return HashString(input);
    }

    public string TransformQueryParameter(string parameter, string value)
    {
        string combined = $"{parameter}:{value}";
        return HashString(combined);
    }

    private string HashString(string input)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }

            return builder.ToString().Substring(0, 12) + "-" + builder.ToString().Substring(12, 8);
        }
    }
}