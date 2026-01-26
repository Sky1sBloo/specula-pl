FROM node:18-slim AS build
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
RUN npm run build

FROM node:18-slim
WORKDIR /app
COPY --from=build /app/dist ./dist
COPY --from=build /app/node_modules ./node_modules
COPY package*.json ./

# Install serve for static file hosting
RUN npm install -g serve

ENV PORT=4173
EXPOSE 4173

CMD ["serve", "-s", "dist", "-l", "4173"]
