// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Econolite.Ode.Messaging
{
    public class KafkaConfigOptions
    {
        public string GroupId { get; set; } = string.Empty;
        public Bootstrap bootstrap { get; set; } = new();
        public Ssl ssl { get; set; } = new();
        public Security security { get; set; } = new();
        public Sasl sasl { get; set; } = new();
    }

    public class Bootstrap
    {
        public string servers { get; set; } = string.Empty;
    }

    public class Ssl
    {
        public string ca { get; set; } = string.Empty;
        public string certificate { get; set; } = string.Empty;
    }

    public class Security
    {
        public string protocol { get; set; } = string.Empty;
    }

    public class Sasl
    {
        public string mechanism { get; set; } = string.Empty;
        public string username { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
    }

    /// <summary>
    /// This is being used to select a different KafkaConfig based on the types TKey and TValue
    /// This is only intended for the case where we have to connect to a second Kafka server and
    /// are attempting to use a different section of the configuration instead of "Kafka" to 
    /// define that connection.
    /// <b>Note the protections surrounding the files used for the "certs" are no longer ensured if two classes use the same file names</b>
    /// </summary>
    /// <typeparam name="TKey">Used in selecting the KafkaConfigOptions in DI</typeparam>
    /// <typeparam name="TValue">Used in selecting the KafkaConfigOptions in DI</typeparam>
    public class KafkaConfigOptions<TKey, TValue> : KafkaConfigOptions
    {
        public CertFilenames CertFilenames { get; set; } = new();
    }

    public class CertFilenames
    {
        public string CAFilename { get; set; } = "secondary.ca.crt";
        public string CertFilename { get; set; } = "secondary.cert.crt";

        public static readonly CertFilenames Default = new CertFilenames
        {
            CAFilename = Consts.CA_CERT_FILE,
            CertFilename = Consts.CLIENT_CERT_FILE,
        };
    }
}
