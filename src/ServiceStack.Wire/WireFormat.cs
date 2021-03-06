﻿namespace ServiceStack.Wire
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using global::Wire;
    using ServiceStack.Web;

    public class WireFormat : IPlugin, IWirePlugin
    {
        public static Serializer WireSerializer = new Serializer(new SerializerOptions(
            versionTolerance:true, 
            preserveObjectReferences:false, 
            surrogates:null, 
            serializerFactories:null, 
            knownTypes:null));

        public void Register(IAppHost appHost)
        {
            appHost.ContentTypes.Register(MimeTypes.Wire,
                Serialize,
                Deserialize);
        }

        public static void Serialize(IRequest requestContext, object dto, Stream outputStream)
        {
            Serialize(dto, outputStream);
        }

        public static void Serialize(object dto, Stream outputStream)
        {
            if (dto == null) return;
            try
            {
                WireSerializer.Serialize(dto, outputStream);
            }
            catch (Exception ex)
            {
                HandleException(ex, dto.GetType());
            }
        }

        public static object Deserialize(Type type, Stream fromStream)
        {
            try
            {
                return WireSerializer.Deserialize(fromStream);
            }
            catch (Exception ex)
            {
                return HandleException(ex, type);
            }
        }

        /// <summary>
        /// MsgPack throws an exception for empty DTO's - normalizing the behavior to 
        /// follow other types and return an empty instance.
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object HandleException(Exception ex, Type type)
        {
            if (ex is SerializationException
                && ex.Message.Contains("does not have any serializable fields nor properties"))
                return type.CreateInstance();

            throw ex;
        }
    }
}
