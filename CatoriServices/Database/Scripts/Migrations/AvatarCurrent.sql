CREATE TABLE IF NOT EXISTS CurrentAvatar (
    CurrentAvatarId INTEGER PRIMARY KEY CHECK(CurrentAvatarId = 1),
    AvatarName TEXT NOT NULL
);
