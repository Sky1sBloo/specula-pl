# Multi-stage Dockerfile for Specula
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS parser-build
WORKDIR /parser
COPY parser/ .
RUN dotnet restore
RUN dotnet publish -c Release -o out

# Main image
FROM ubuntu:22.04

# Install dependencies
RUN apt-get update && apt-get install -y \
    build-essential \
    cmake \
    nodejs \
    npm \
    dotnet-runtime-8.0 \
    curl \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app

# Copy and build C++ lexer
COPY src/ ./src/
COPY CMakeLists.txt .
RUN cmake -B build -DCMAKE_BUILD_TYPE=Release
RUN cmake --build build --config Release

# Copy lexer backend
COPY lexer/ ./lexer/
WORKDIR /app/lexer
RUN npm install

# Copy parser from build stage
WORKDIR /app/parser
COPY --from=parser-build /parser/out ./

# Copy and build frontend
WORKDIR /app/frontend
COPY frontend/package*.json ./
RUN npm install
COPY frontend/ ./
RUN npm run build

# Create startup script
WORKDIR /app
RUN echo '#!/bin/bash\n\
cd /app/parser && dotnet SpeculaSyntaxAnalyzer.dll &\n\
cd /app/lexer && npm start &\n\
cd /app/frontend && npm run preview -- --host 0.0.0.0 --port $PORT\n\
wait' > start.sh && chmod +x start.sh

EXPOSE 3001 5000 4173

CMD ["./start.sh"]
