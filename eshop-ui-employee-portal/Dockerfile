FROM node:25.4.0-alpine3.23  AS development
WORKDIR /app
RUN npm install -g @angular/cli@21.0.5
COPY package.json package-lock.json ./
RUN npm ci
COPY . .
EXPOSE 4202 49153
ENTRYPOINT ["ng","serve","--host","0.0.0.0","--poll","500"]
CMD ["--configuration","development"]

FROM development AS build
ARG CONFIGURATION="production"
ARG OAUTH_AUTHORIZATION_ENDPOINT_PATH="/authorize"
ARG OAUTH_CLIENT_ID="employee-portal"
ARG OAUTH_SCOPE="openid roles api offline_access"
# SETUP OATH SETTINGS
RUN sed -i  "s|__OAUTH_AUTHORIZATION_ENDPOINT_PATH__|${OAUTH_AUTHORIZATION_ENDPOINT_PATH}|g" /app/src/environments/environment.ts
RUN sed -i  "s|__OAUTH_CLIENT_ID__|${OAUTH_CLIENT_ID}|g" /app/src/environments/environment.ts
RUN sed -i  "s|__OAUTH_SCOPE__|${OAUTH_SCOPE}|g" /app/src/environments/environment.ts
RUN ng build --configuration=$CONFIGURATION

FROM nginx:alpine3.17 AS release
COPY --from=build /app/.nginx/nginx.conf /etc/nginx/conf.d/default.conf
WORKDIR /usr/share/nginx/html
# Remove default nginx static assets
RUN rm -rf ./*
# Copy static assets from builder stage
COPY --from=build /app/dist/eshop-ui-employee-portal/browser .
# Containers run nginx with global directives and daemon off
EXPOSE 80
ENTRYPOINT ["nginx", "-g", "daemon off;"]
