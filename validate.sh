#!/bin/bash

set -u

echo "======================================"
echo "VALIDAÇÃO DO AMBIENTE BOLEIROFFICE"
echo "======================================"

echo ""
echo "1. Verificando serviço da API..."
systemctl is-active --quiet boleiroffice-api
if [ $? -eq 0 ]; then
    echo "✅ API está rodando"
else
    echo "❌ API NÃO está rodando"
    systemctl status boleiroffice-api --no-pager || true
fi

echo ""
echo "2. Verificando porta da API..."
ss -tulnp | grep :5001 > /dev/null
if [ $? -eq 0 ]; then
    echo "✅ Porta 5001 ativa"
else
    echo "❌ Porta 5001 NÃO está ativa"
fi

echo ""
echo "3. Testando API local..."
API_RESPONSE=$(curl -s --max-time 10 http://127.0.0.1:5001/api/empresas)
if [[ "$API_RESPONSE" == *"items"* ]]; then
    echo "✅ API local OK"
else
    echo "❌ API local falhou"
    echo "$API_RESPONSE"
fi

echo ""
echo "4. Verificando Nginx..."
systemctl is-active --quiet nginx
if [ $? -eq 0 ]; then
    echo "✅ Nginx rodando"
else
    echo "❌ Nginx NÃO está rodando"
fi

echo ""
echo "5. Validando config do Nginx..."
nginx -t > /dev/null 2>&1
if [ $? -eq 0 ]; then
    echo "✅ Config Nginx OK"
else
    echo "❌ Config Nginx inválida"
    nginx -t || true
fi

echo ""
echo "6. Testando frontend (localhost)..."
FRONT_RESPONSE=$(curl -s --max-time 10 http://localhost)
if [[ "$FRONT_RESPONSE" == *"<html"* ]]; then
    echo "✅ Frontend servido pelo Nginx"
else
    echo "❌ Frontend NÃO está sendo servido"
fi

echo ""
echo "7. Testando API via Nginx (/api)..."
PROXY_RESPONSE=$(curl -s --max-time 10 http://localhost/api/empresas)
if [[ "$PROXY_RESPONSE" == *"items"* ]]; then
    echo "✅ Proxy /api funcionando"
else
    echo "❌ Proxy /api NÃO está funcionando"
    echo "$PROXY_RESPONSE"
fi

echo ""
echo "8. Verificando arquivos do frontend..."
if [ -f /var/www/boleiroffice-front/index.html ]; then
    echo "✅ index.html encontrado"
else
    echo "❌ index.html NÃO encontrado"
fi

echo ""
echo "9. Verificando permissões..."
stat -c "%U %G" /var/www/boleiroffice-front 2>/dev/null | grep www-data > /dev/null
if [ $? -eq 0 ]; then
    echo "✅ Permissões OK (www-data)"
else
    echo "⚠️ Permissões podem estar incorretas"
fi

echo ""
echo "10. Teste externo (IP)..."
IP_RESPONSE=$(curl -s --max-time 10 http://82.29.58.99)
if [[ "$IP_RESPONSE" == *"<html"* ]]; then
    echo "✅ Acesso externo OK"
else
    echo "❌ Acesso externo NÃO está funcionando"
    echo "$IP_RESPONSE"
fi

echo ""
echo "======================================"
echo "FIM DA VALIDAÇÃO"
echo "======================================"