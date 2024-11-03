# Refrase (**RE**verse **FRA**me **SE**arch)

Refrase is a small tool that indexes videos and allows performing reverse searches for individual frames.
It stores its data in a local SQLite database and provides a simple JSON REST API to interact with it.

## Web UI

Under `/` is a very basic web UI that shows the current ingestion queue and all ingested videos.

## API endpoints

The endpoints are exposed via Swagger under `/swagger`.
There is no authentication, authorization, rate limiting or other stuff you might want
when exposing an API in the internet as Refrase is meant to be self-hosted.

### GET `/api/health`

Returns HTTP 200 and `OK` in the body if the service is running.

### POST `/api/video`

Allows ingesting a new video and analyzes its frames.
The video content should be sent in the body while metadata is provided via query parameters:

| Name       | Content                       | Example value                                   | Required | Default     |
|------------|-------------------------------|-------------------------------------------------|----------|-------------|
| `name`     | The display name of the video | `"Rick Astley - Never Gonna Give You Up"`       | Yes      | n/a         |
| `category` | The category of the video     | `"Music Video"`                                 | No       | `"Unknown"` |
| `url`      | A URL pointing to the video   | `"https://www.youtube.com/watch?v=dQw4w9WgXcQ"` | No       | `null`      |

Example call:

```
curl -X 'POST' \
  'http://localhost:5000/api/video' \
  -H 'Content-Type: multipart/form-data' \
  -F 'Name=Jellyfish' \
  -F 'Category=Clips' \
  -F 'Url=https://test-videos.co.uk/jellyfish/mp4-h264' \
  -F 'Video=@res/video.mp4;type=video/mp4'
```

### POST `/api/frame/search`

Searches through all analyzed frames and returns the best match.
The frame should be sent in the body.

Example call:

```
curl http://localhost:5000/api/frame/search --data-binary res/@frame8.jpg
```

The response looks like this:
```
{
  "match": {
    "video": {
      "name": "Jellyfish",
      "category": "Clips",
      "url": "https://test-videos.co.uk/jellyfish/mp4-h264"
    },
    "frame": {
      "index": 253,
      "time": "00:00:08.4417753",
      "hash": 10719801021238211282,
      "similarity": 1
    }
  },
  "inputHash": 10719801021238211282
}
```

## How does it work?

When ingesting a video it is split into individual frames, each of which is hashed using [ImageHash](https://github.com/coenm/ImageHash).
When searching for a frame it is also hashed and the hash is compared to the hashes of all frames of all ingested videos.
The frame with the shorted [hamming distance](https://en.wikipedia.org/wiki/Hamming_distance) is returned as the result.

## Changelog

see [CHANGELOG.md](https://github.com/koryphaee/Refrase/blob/main/CHANGELOG.md)
