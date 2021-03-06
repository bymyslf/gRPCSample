// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: hello.proto
#pragma warning disable 1591
#region Designer generated code

using System;
using System.Threading;
using System.Threading.Tasks;
using grpc = global::Grpc.Core;

namespace gRPCSample.Proto {
  public static partial class HelloService
  {
    static readonly string __ServiceName = "gRPCSample.HelloService";

    static readonly grpc::Marshaller<global::gRPCSample.Proto.HelloRequest> __Marshaller_HelloRequest = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::gRPCSample.Proto.HelloRequest.Parser.ParseFrom);
    static readonly grpc::Marshaller<global::gRPCSample.Proto.HelloResponse> __Marshaller_HelloResponse = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::gRPCSample.Proto.HelloResponse.Parser.ParseFrom);

    static readonly grpc::Method<global::gRPCSample.Proto.HelloRequest, global::gRPCSample.Proto.HelloResponse> __Method_SayHello = new grpc::Method<global::gRPCSample.Proto.HelloRequest, global::gRPCSample.Proto.HelloResponse>(
        grpc::MethodType.Unary,
        __ServiceName,
        "SayHello",
        __Marshaller_HelloRequest,
        __Marshaller_HelloResponse);

    /// <summary>Service descriptor</summary>
    public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
    {
      get { return global::gRPCSample.Proto.HelloReflection.Descriptor.Services[0]; }
    }

    /// <summary>Base class for server-side implementations of HelloService</summary>
    public abstract partial class HelloServiceBase
    {
      public virtual global::System.Threading.Tasks.Task<global::gRPCSample.Proto.HelloResponse> SayHello(global::gRPCSample.Proto.HelloRequest request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

    }

    /// <summary>Client for HelloService</summary>
    public partial class HelloServiceClient : grpc::ClientBase<HelloServiceClient>
    {
      /// <summary>Creates a new client for HelloService</summary>
      /// <param name="channel">The channel to use to make remote calls.</param>
      public HelloServiceClient(grpc::Channel channel) : base(channel)
      {
      }
      /// <summary>Creates a new client for HelloService that uses a custom <c>CallInvoker</c>.</summary>
      /// <param name="callInvoker">The callInvoker to use to make remote calls.</param>
      public HelloServiceClient(grpc::CallInvoker callInvoker) : base(callInvoker)
      {
      }
      /// <summary>Protected parameterless constructor to allow creation of test doubles.</summary>
      protected HelloServiceClient() : base()
      {
      }
      /// <summary>Protected constructor to allow creation of configured clients.</summary>
      /// <param name="configuration">The client configuration.</param>
      protected HelloServiceClient(ClientBaseConfiguration configuration) : base(configuration)
      {
      }

      public virtual global::gRPCSample.Proto.HelloResponse SayHello(global::gRPCSample.Proto.HelloRequest request, grpc::Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        return SayHello(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual global::gRPCSample.Proto.HelloResponse SayHello(global::gRPCSample.Proto.HelloRequest request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_SayHello, null, options, request);
      }
      public virtual grpc::AsyncUnaryCall<global::gRPCSample.Proto.HelloResponse> SayHelloAsync(global::gRPCSample.Proto.HelloRequest request, grpc::Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        return SayHelloAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual grpc::AsyncUnaryCall<global::gRPCSample.Proto.HelloResponse> SayHelloAsync(global::gRPCSample.Proto.HelloRequest request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_SayHello, null, options, request);
      }
      /// <summary>Creates a new instance of client from given <c>ClientBaseConfiguration</c>.</summary>
      protected override HelloServiceClient NewInstance(ClientBaseConfiguration configuration)
      {
        return new HelloServiceClient(configuration);
      }
    }

    /// <summary>Creates service definition that can be registered with a server</summary>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    public static grpc::ServerServiceDefinition BindService(HelloServiceBase serviceImpl)
    {
      return grpc::ServerServiceDefinition.CreateBuilder()
          .AddMethod(__Method_SayHello, serviceImpl.SayHello).Build();
    }

  }
}
#endregion
