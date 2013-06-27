#!/usr/bin/perl
#create Gource video

if (-d ".hg") {
    do("FetchHgGravatars.pl");
    system("gource -1280x720 -o gource.ppm -user-image-dir .hg/avatar/");
	system("ffmpeg -y -r 60 -f image2pipe -vcodec ppm -i gource.ppm -vcodec libx264 -preset ultrafast -pix_fmt yuv420p -crf 1 -threads 0 -bf 0 gource.mp4");
}
elsif (-d ".git") {
    do("FetchGitGravatars.pl");
    system("gource -1280x720 -o gource.ppm -user-image-dir .git/avatar/");
	system("ffmpeg -y -r 60 -f image2pipe -vcodec ppm -i gource.ppm -vcodec libx264 -preset ultrafast -pix_fmt yuv420p -crf 1 -threads 0 -bf 0 gource.mp4");
}
else {
	die("no .hg/ or .git/ folder in current path!\n")
}