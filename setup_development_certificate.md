# Generate development-certificate

## 1. Generate openssl-key

```bash
openssl genrsa -des3 -out rootCA.key 2048
```

## 2. Generate Root-SSL-Certificate

```bash
openssl req -x509 -new -nodes -key rootCA.key -sha256 -days 1024 -out rootCA.pem
```

## 3. Genrate Domain-Certificate

### 3.1 Create server-key

```bash
openssl req -new -sha256 -nodes -out server.csr -newkey rsa:2048 -keyout server.key
```

### 3.2 Create ext-config

```config
authorityKeyIdentifier=keyid,issuer
basicConstraints=CA:FALSE
keyUsage = digitalSignature, nonRepudiation, keyEncipherment, dataEncipherment
subjectAltName = @alt_names

[alt_names]
DNS.1 = localhost
```

### 3.3 Create Certificate using key and config

```bash
openssl x509 -req -in server.csr -CA rootCA.pem -CAkey rootCA.key -CAcreateserial -out server.crt -days 500 -sha256 -extfile v3.ext
```

### 3.4 Append certificate and key (Used in Kestrel-Configuration)

```bash
openssl pkcs12 -export -inkey server.key  -in server.crt -name my_name -out server.p12
```

## 4. Trust certficate

Tu use this certificate, you have to trust it. For that, we have to set up a nginx-instance and use the certificate in the ssl-configuration.

Replace the contents of the file /etc/nginx/sites-enabled/default with the following (**NOTE**: specify the correct path to your key and certificate):

```nginx
server {
    listen 443 ssl default_server;
    listen [::]:443 ssl default_server;
    ssl on;
    ssl_certificate /path/to/your/cert/server.crt;
    ssl_certificate_key /path/to/your/key/server.key;
    root /var/www/html;

    index index.html index.htm index.nginx-debian.html;

    server_name _;

    location / {
        try_files $uri $uri/ =404;
    }
}
```

Now start the nginx-service and connect to <https://localhost>.

Done!