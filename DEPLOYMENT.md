# Railway Deployment Guide

## Prerequisites
- GitHub account
- Railway account (sign up at https://railway.app)
- Your code pushed to GitHub

## Deployment Steps

### 1. Push to GitHub
```bash
git add .
git commit -m "Add Railway deployment configuration"
git push origin dev
```

### 2. Create Railway Project

1. Go to https://railway.app
2. Click "Start a New Project"
3. Select "Deploy from GitHub repo"
4. Authorize Railway to access your GitHub
5. Select your `specula-pl` repository
6. Select the `dev` branch

### 3. Configure Environment Variables

In Railway dashboard, add these environment variables:

- `PORT` = `4173` (Railway will auto-assign, but frontend needs this)
- `PARSER_API_URL` = `http://localhost:5000/api/analyze`

### 4. Deploy

Railway will automatically:
1. Detect the Dockerfile
2. Build the container
3. Deploy all services
4. Provide you with a public URL

### 5. Update Frontend API URLs

After deployment, you'll need to update the API URLs in your frontend to use the Railway domain:

In `frontend/src/lib/lexer.ts` and `frontend/src/lib/parser.ts`, change:
- `http://localhost:3001` → `https://your-app.railway.app` (or use relative URLs)

## Architecture

Railway will run:
- **Frontend** (Vite) on port 4173
- **Lexer API** (Node.js) on port 3001
- **Parser API** (.NET) on port 5000

All in a single container.

## Troubleshooting

### Build Fails
- Check Railway logs in the dashboard
- Verify all dependencies are in package.json
- Ensure CMakeLists.txt is correct

### Services Not Starting
- Check that all ports are correct
- Verify environment variables
- Check Railway logs for errors

### API Connection Issues
- Update CORS settings in backend
- Verify API URLs in frontend
- Check that all services are running

## Alternative: Separate Services

If the single container approach has issues, you can deploy as 3 separate Railway services:

1. **Service 1: Frontend**
   - Root: `frontend/`
   - Build: `npm install && npm run build`
   - Start: `npm run preview`

2. **Service 2: Lexer**
   - Root: `lexer/`
   - Build: Compile C++ + npm install
   - Start: `npm start`

3. **Service 3: Parser**
   - Root: `parser/`
   - Build: `dotnet restore && dotnet build`
   - Start: `dotnet run`

Then link them via environment variables.
