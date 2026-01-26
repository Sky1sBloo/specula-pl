# Build stage for C++ lexer
FROM ubuntu:22.04 AS cpp-build

RUN apt-get update && apt-get install -y \
    build-essential \
    cmake \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY src/ ./src/
COPY CMakeLists.txt .
RUN cmake -B build -DCMAKE_BUILD_TYPE=Release
RUN cmake --build build --config Release

# Runtime stage
FROM node:18-slim

WORKDIR /app

# Copy compiled lexer
COPY --from=cpp-build /app/build/specula /app/specula

# Copy and install lexer server
COPY lexer/package*.json ./
RUN npm install --production

COPY lexer/ ./

# Make lexer executable
RUN chmod +x /app/specula

ENV PORT=3001
ENV LEXER_EXECUTABLE=/app/specula

EXPOSE 3001

CMD ["node", "server.js"]
