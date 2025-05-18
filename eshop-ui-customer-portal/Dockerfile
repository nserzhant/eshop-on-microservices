FROM node:22.4.0-alpine3.20  as development
WORKDIR /app
RUN npm install -g @angular/cli@18.1.0
COPY package.json package-lock.json ./
RUN npm ci
COPY . .
EXPOSE 4201 49153
ENTRYPOINT ["ng","serve","--host","0.0.0.0","--poll","500"]
CMD ["--configuration","development"]

FROM development AS build
ARG CONFIGURATION="production"
RUN ng build --configuration=$CONFIGURATION

FROM nginx:alpine3.17 as release
COPY --from=build /app/.nginx/nginx.conf /etc/nginx/conf.d/default.conf
WORKDIR /usr/share/nginx/html
# Remove default nginx static assets
RUN rm -rf ./*
# Copy static assets from builder stage
COPY --from=build /app/dist/eshop-ui-customer-portal/browser .
# Containers run nginx with global directives and daemon off
EXPOSE 80
ENTRYPOINT ["nginx", "-g", "daemon off;"]


