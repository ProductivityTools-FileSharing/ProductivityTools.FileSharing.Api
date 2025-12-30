If server returns 413 Request Entity Too Large - File Upload Issue

Then

```
nano /etc/nginx/nginx.conf

client_max_body_size 100M;

sudo nginx -t
sudo nginx -s reload
sudo systemctl restart nginx

```

https://stackoverflow.com/questions/24306335/413-request-entity-too-large-file-upload-issue