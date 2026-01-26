# Railway Multi-Service Deployment Guide

## Quick Setup (3 Services)

### Service 1: Lexer (Node.js + C++)
1. **Add Service** → GitHub Repo → `specula-pl`
2. **Root Directory**: Leave empty (uses root Dockerfile)
3. **Environment Variables**:
   - `PARSER_API_URL` = (add after parser is deployed)

### Service 2: Parser (.NET)
1. **Add Service** → GitHub Repo → `specula-pl`
2. **Root Directory**: `parser`
3. Uses `parser/Dockerfile`

### Service 3: Frontend (Vite)
1. **Add Service** → GitHub Repo → `specula-pl`
2. **Root Directory**: `frontend`
3. **Environment Variables**:
   - `VITE_LEXER_API_URL` = `https://[lexer-url]/lex`
   - `VITE_PARSER_API_URL` = `https://[lexer-url]/parse`

---

## After All Services Deploy

1. Get the URLs for each service from Railway dashboard
2. Update **Lexer** → `PARSER_API_URL` = `https://[parser-url]/api/analyze`
3. Update **Frontend** → `VITE_LEXER_API_URL` and `VITE_PARSER_API_URL`
4. Redeploy services

## Troubleshooting

- **Build fails**: Check Railway logs, verify Dockerfile paths
- **API errors**: Ensure URLs use `https://` and end correctly
- **CORS issues**: Frontend and backends must allow cross-origin requests
