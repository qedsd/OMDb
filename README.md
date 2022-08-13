# OMDb
一个本地影视资源管理器

---

# 名词

## 词条

类似于字典词条、维基词条，软件用词条代表着一个电影、电视、以及任何你想添加进软件进行管理的东西，可以简单理解为词条=电影/电视。每个词条都会有自己的独立文件夹存放词条专属文件，如视频、图片等等可以在词条详细页看到、管理的文件、信息。

![Img](https://github.com/qedsd/OMDb/blob/master/Img/entryfolder.png?raw=true?raw=true)
- Img 图片文件
- Resource BT之类的用于下载资源的文件
- Sub 字幕文件
- Video 视频文件
- metadata.json 词条元数据，存放着词条一些基本信息，如词条名、描述、可扩展的评分等。

当然，这些文件夹的存放类型描述只是规范下的情况，并没有规定各个文件夹下只能存放什么后缀名的文件，实际存放什么取决于你。但是需要注意，Img文件夹下的内容，在软件的词条详情页只会展示其内包含的格式受支持的图片文件。

特别注明：词条可以存在多个名称，在词条详细页可编辑。词条文件夹名称与词条默认名一致。

## 标签

![Img](https://github.com/qedsd/OMDb/blob/master/Img/whatislabel.png?raw=true?raw=true)

标记词条的分类，主要用于区分、筛选词条，非核心功能，完全可以不用。此外，词条是电影、电视等的统称，本身是不可以区分电影、电视的，而是通过标签来附加属性进行区分。

## 仓库

前面提到，每个词条都有自己的一个独立的文件夹，存在很多个词条的时候，文件夹数量肯定也是非常多的，所以需要把这些词条文件夹存放到某个文件夹下集中管理，而这个某个文件夹即是一个仓库，所以，创建词条前必须先创建仓库。

考虑到可能需要把词条存放到不同的硬盘、不同的硬盘分区，所以仓库是可以有多个的，每个仓库只维护自己文件夹内的词条。

![Img](https://github.com/qedsd/OMDb/blob/master/Img/storagefolder.png?raw=true?raw=true)

- Entries 此仓库包含的词条，默认这个文件夹内的每个文件夹对应着各个词条文件夹
- OMDb.db 一个SQLite数据库文件，记录仓库内词条名称、每个词条相对路径、标签、观看记录等数据。

---

# 文件

## OMDb.db
词条存放于仓库之下，仓库如何知道它拥有着哪些词条及各个词条的名称、观看记录，以及最重要的：词条的完整文件夹路径。OMDb.db这个数据库文件就是用来记录这些词条信息的，总不能每次都要扫一遍仓库文件夹内的所有文件夹来显示它自己拥有着哪些词条吧。数据库内记录的数据，可以便于词条页进行筛选，快速定位词条。

## metadata.json
记录词条元数据的文件，存放着词条一些基本信息，如词条名、描述、可扩展的评分等。OMDb.db既然记录了仓库内各个词条数据，为什么还要在词条文件夹下单独建个文件来记录词条的一些信息？并且还有些重复信息？

OMDb.db固然完全可以取代metadata.json，但是OMDb.db是一个数据库文件，依赖于程序来理解的，也就是说离不开OMDb这个软件。并且，本着尽可能减少对OMDb的依赖，将一些基本信息、不需要用于筛选的、只需要查看词条详情时才需要加载的数据单独抽出来存放到每个词条文件夹内是一个不错的想法。以后如果不想使用OMDb这个软件了，问题也不大，完全可以自己用记事本打开metadata.json读取信息。

## 最根本的不同点
需要参与筛选、搜索之类的数据，放到OMDb.db。只在查看词条详情时才需要加载的，放到metadata.json。

---

# 使用

1. 仓库页创建仓库
2. 标签页创建标签（非必须）
3. 主页创建词条
4. 词条页打开词条详情
5. 词条详细页添加更多信息，如描述、观看记录、各种文件等（文件可以鼠标拖拽进相应区域进行复制）


---

# 软件截图

![Img](https://github.com/qedsd/OMDb/blob/master/Img/entry.png?raw=true?raw=true?raw=true)
![Img](https://github.com/qedsd/OMDb/blob/master/Img/entrydetail1.png?raw=true?raw=true?raw=true)
![Img](https://github.com/qedsd/OMDb/blob/master/Img/entrydetail2.png?raw=true?raw=true?raw=true)
![Img](https://github.com/qedsd/OMDb/blob/master/Img/storage.png?raw=true?raw=true?raw=true)
![Img](https://github.com/qedsd/OMDb/blob/master/Img/label.png?raw=true?raw=true?raw=true)
