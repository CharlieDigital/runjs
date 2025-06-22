export function useBackend() {
  const secretId = ref("none");
  const secretValue = ref("Abracadabra!");
  const prompt = ref("");
  const result = ref("");
  const executing = ref(false);
  const platform = ref("OpenAI");
  const service = ref("httpbin");

  watch(
    secretId,
    (id) => {
      if (id === "none") {
        return;
      }

      prompt.value = prompt.value.replace(/runjs:secret:[\w]+/g, id);
    },
    { immediate: true }
  );

  watch(
    service,
    (svc) => {
      if (svc === "jsonplaceholder") {
        prompt.value =
          "Generate some JavaScript that will POST to https://jsonplaceholder.typicode.com/posts/. " +
          'Create a new post: { "title": "Hello", "body": "runjs:secret:SECRET_GUID_HERE", "userId": 1 }. ' +
          "Include the Authorization header with the secret key runjs:secret:SECRET_GUID_HERE. " +
          'Return whether the JSON contains the phrase "abracadabra" anywhere in the response. ';
      } else {
        prompt.value =
          "Generate some JavaScript that will POST to https://httpbin.org/post. " +
          "Use the the authorization header with value: runjs:secret:SECRET_GUID_HERE " +
          "Just return the raw response; do not modify it; do not add your commentary. " +
          "Give me the full response directly as text.";
      }
    },
    { immediate: true }
  );

  async function saveSecret() {
    if (!secretValue.value) return;

    try {
      executing.value = true;

      const response = await fetch("/api/store-secret", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({ secret: secretValue.value }),
      });

      if (response.ok) {
        secretId.value = await response.text();

        console.log("Secret saved:", secretId.value);
      } else {
        Notify.create({
          type: "negative",
          message: "Failed to store secret value.",
        });
      }
    } finally {
      executing.value = false;
    }
  }

  async function executePrompt() {
    if (!prompt.value) return;

    try {
      executing.value = true;
      result.value = ""; // Clear previous result

      const response = await fetch("/api/run-prompt", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          prompt: prompt.value,
          platform: platform.value,
        }),
      });

      if (response.ok) {
        result.value = await response.text();
      } else {
        Notify.create({
          type: "negative",
          message: "Failed to execute prompt.",
        });
      }
    } finally {
      executing.value = false;
    }
  }

  return {
    saveSecret,
    executePrompt,
    secretId,
    secretValue,
    prompt,
    result,
    executing,
    platform,
    service,
  };
}
