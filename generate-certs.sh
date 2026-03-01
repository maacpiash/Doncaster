#!/usr/bin/env bash
set -euo pipefail

CERT_DIR="$(dirname "$0")/docker/certs"
CERT_PASSWORD="${CERT_PASSWORD:-changeit}"
mkdir -p "$CERT_DIR"

if [ -f "$CERT_DIR/cert.pfx" ]; then
    echo "Certificate already exists in $CERT_DIR"
    exit 0
fi

echo "Generating self-signed certificate..."
openssl req -x509 -nodes -days 365 \
    -newkey rsa:2048 \
    -keyout "$CERT_DIR/key.pem" \
    -out "$CERT_DIR/cert.pem" \
    -subj "/CN=localhost" \
    -addext "subjectAltName=DNS:localhost,IP:127.0.0.1"

openssl pkcs12 -export \
    -out "$CERT_DIR/cert.pfx" \
    -inkey "$CERT_DIR/key.pem" \
    -in "$CERT_DIR/cert.pem" \
    -password "pass:$CERT_PASSWORD"

echo "Certificate generated in $CERT_DIR (PFX password: $CERT_PASSWORD)"
