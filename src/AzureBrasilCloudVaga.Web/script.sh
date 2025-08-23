#!/bin/sh
echo "Executando script customizado antes do NGINX iniciar..."

JSON_FILE="/usr/share/nginx/html/appsettings.json"

echo "Atualiza o JSON usando variáveis de ambiente do container"

json_array=$(printf '%s' "$NEW_SCOPE" | tr ',' '\n' | jq -R . | jq -s .)

jq --arg authority "$AZURE_AUTHORITY" \
   --arg clientId "$CLIENT_ID" \
   --arg baseUrl "$API_BASEURL" \
   --argjson scopes "$json_array" \
   '.AzureAd.Authority = $authority
    | .AzureAd.ClientId = $clientId
    | .Api.BaseUrl = $baseUrl
    | .Api.Scopes = $scopes' \
   "$JSON_FILE" > "${JSON_FILE}.tmp" && mv "${JSON_FILE}.tmp" "$JSON_FILE"

echo "appsettings.json atualizado com sucesso!"