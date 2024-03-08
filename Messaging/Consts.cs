// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;

namespace Econolite.Ode.Messaging;

public class Consts
{
    // The topics should not be here they are not common they belong with the individual message assemblies.
    public static readonly string TOPICS_DEVICESTATUS = "Topics:DeviceStatus";
    public static readonly string TOPICS_PCSTATUS = "Topics:PavementConditionStatus";
    public static readonly string TOPICS_CV_BSM = "Topics:OdeBsm";
    public static readonly string TOPICS_CV_TIM = "Topics:OdeTim";
    public static readonly string TOPICS_CV_SPAT = "Topics:OdeSpat";
    public static readonly string TOPICS_CV_SRM = "Topics:OdeSrm";
    public static readonly string TOPICS_CV_EVERY_MIN = "Topics:MinuteTopic";
    public static readonly string TOPICS_WrongWayDriverStatus = "Topics:WrongWayDriverStatus";
    public static readonly string TOPICS_DM_CONFIG_REQUEST = "Topics:DeviceManagerConfigRequest";
    public static readonly string TOPICS_DM_CONFIG_RESPONSE = "Topics:DeviceManagerConfigResponse";

    public static readonly string TENANT_ID_HEADER = "tenantId";
    public static readonly string DEVICE_ID_HEADER = "deviceId";
    public static readonly string TYPE_HEADER = "type";
    public static readonly string TYPE_UNSPECIFIED = "unspecified";
    [Obsolete]
    public static readonly string KAFKA_GROUP_ID = "Kafka:GroupId";
    [Obsolete]
    public static readonly string KAFKA_BOOTSTRAP_SERVERS = "Kafka:bootstrap:servers";
    [Obsolete]
    public static readonly string KAFKA_SECURITY_PROTOCOL = "Kafka:security:protocol";
    [Obsolete]
    public static readonly string KAFKA_SASL_MECHANISM = "Kafka:sasl:mechanism";
    [Obsolete]
    public static readonly string KAFKA_SASL_USERNAME = "Kafka:sasl:username";
    [Obsolete]
    public static readonly string KAFKA_SASL_PASSWORD = "Kafka:sasl:password";
    [Obsolete]
    public static readonly string CONSUMER_PARTITION = "Kafka:ConsumerPartition";
    [Obsolete]
    public static readonly string CA_CERT = "Kafka:ssl:ca";
    [Obsolete]
    public static readonly string CLIENT_CERT = "Kafka:ssl:certificate";

    public static readonly string CA_CERT_FILE = "ca.crt";
    public static readonly string CLIENT_CERT_FILE = "client.crt";
}
