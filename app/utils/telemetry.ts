import { OTLPTraceExporter } from "@opentelemetry/exporter-trace-otlp-grpc";
import { resourceFromAttributes } from "@opentelemetry/resources";
import {
  NodeTracerProvider,
  SimpleSpanProcessor,
  TraceIdRatioBasedSampler,
} from "@opentelemetry/sdk-trace-node";
import {
  ATTR_SERVICE_NAME,
  ATTR_SERVICE_VERSION,
} from "@opentelemetry/semantic-conventions";

// OpenTelemetry configuration for visibility (http://localhost:18888)
const traceProvider = new NodeTracerProvider({
  spanProcessors: [new SimpleSpanProcessor(new OTLPTraceExporter())],
  resource: resourceFromAttributes({
    [ATTR_SERVICE_NAME]: "runjs-nuxt",
    [ATTR_SERVICE_VERSION]: "1.0.0",
  }),
  sampler: new TraceIdRatioBasedSampler(1),
});

traceProvider.register();

export { traceProvider };
