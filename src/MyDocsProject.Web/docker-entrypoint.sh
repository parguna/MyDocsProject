#!/bin/bash
set -e

# Persisted on the openiddict_certs named volume (mounted as a directory at /app/keys —
# see the Dockerfile comment for why it can't be mounted directly onto a single file path).
CERT_PATH=/app/keys/openiddict.pfx

# -s: file exists and is non-empty, so this is true only on a genuine first run (or if the
# persisted volume was reset). On every later container start the existing cert is reused,
# so restarts never invalidate already-issued tokens or persisted encrypted data.
if [ ! -s "$CERT_PATH" ]; then
  echo "No persisted certificate found — generating a new self-signed certificate (first run only)..."
  openssl req -x509 -newkey rsa:2048 -keyout /tmp/key.pem -out /tmp/cert.pem -days 3650 -nodes -subj "/CN=MyDocsProject"
  openssl pkcs12 -export -out "$CERT_PATH" -inkey /tmp/key.pem -in /tmp/cert.pem -passout "pass:${AuthServer__CertificatePassPhrase}"
  rm -f /tmp/key.pem /tmp/cert.pem
  echo "Certificate generated at $CERT_PATH"
else
  echo "Reusing existing persisted certificate at $CERT_PATH"
fi

# MyDocsProjectWebModule.cs loads the OpenIddict signing/encryption certificate from the
# literal relative path "openiddict.pfx" (resolved against the app's working directory,
# /app). Symlink it to the persisted file so that lookup succeeds without any source change.
ln -sf "$CERT_PATH" /app/openiddict.pfx

exec dotnet MyDocsProject.Web.dll
