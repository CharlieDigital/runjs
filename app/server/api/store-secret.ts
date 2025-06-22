// Store the secret using the secret service at http://localhost:5000/secret
export default defineEventHandler(async (event) => {
  const { secret } = await readBody(event);

  const response = await fetch(import.meta.env.VITE_SECRETS_ENDPOINT, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({ Value: secret }),
  });

  if (!response.ok) {
    console.error("Failed to store secret:", response.statusText);
    return null;
  }

  const secretId = await response.text();

  console.log("Stored secret with ID:", secretId);

  return secretId;
});
