/**
*  ueditor完整配置项
*  可以在这里配置整个编辑器的特性
*/

/**************************提示********************************
* 所有被注释的配置项的默认值就是注释项后边给的值或者是有提示默认值是多少，如果你想改，就去掉注释，修改默认值，或者在实例化编辑器时传入相应的值，就会覆盖默认的配置值
* 每次升级编辑器时，可以直接用你改的editor_config.js直接覆盖新版本的editor_config.js,而不用担心会出现错误提示
**************************提示********************************/


(function () {
    /**
    * 编辑器资源文件根路径。它所表示的含义是：以编辑器实例化页面为当前路径，指向编辑器资源文件（即dialog等文件夹）的路径。
    * 鉴于很多同学在使用编辑器的时候出现的种种路径问题，此处强烈建议大家使用"相对于网站根目录的相对路径"进行配置。
    * "相对于网站根目录的相对路径"也就是以斜杠开头的形如"/myProject/ueditor/"这样的路径。
    * 如果站点中有多个不在同一层级的页面需要实例化编辑器，且引用了同一UEditor的时候，此处的URL可能不适用于每个页面的编辑器。
    * 因此，UEditor提供了针对不同页面的编辑器可单独配置的根路径，具体来说，在需要实例化编辑器的页面最顶部写上如下代码即可。当然，需要令此处的URL等于对应的配置。
    * window.UEDITOR_HOME_URL = "/xxxx/xxxx/";
    */
    var URL;
    var URL_UPLOAD = "http://image.tlyh.com.gqc/"

    /**
    * 此处配置写法适用于UEditor小组成员开发使用，外部部署用户请按照上述说明方式配置即可，建议保留下面两行，以兼容可在具体每个页面配置window.UEDITOR_HOME_URL的功能。
    */
    var tmp = window.location.pathname;
    URL = "/Content/third/ueditor/"; //window.UEDITOR_HOME_URL || tmp.substr(0, tmp.lastIndexOf("\/") + 1).replace("_examples/", "").replace("website/", ""); //这里你可以配置成ueditor目录在您网站的相对路径或者绝对路径（指以http开头的绝对路径）

    /**
    * 配置项主体。注意，此处所有涉及到路径的配置别遗漏URL变量。
    */
    window.UEDITOR_CONFIG = {

        //为编辑器实例添加一个路径，这个不能被注释
        UEDITOR_HOME_URL: URL

        //indent
        //首行缩进距离,默认是2em
        //,indentValue:'2em'

        //insertimage
        //图片上传配置区
        , imageUrl: URL_UPLOAD + "UploadHandler.ashx?appName=ueditor"       //图片上传提交地址
        , imagePath: URL_UPLOAD                      //图片修正地址，引用了fixedImagePath,如有特殊需求，可自行配置
        , imageFieldName: "upfile"                   //图片描述的key,若此处修改，需要在后台对应文件修改对应参数
        //, compressSide:0                            //等比压缩的基准，确定maxImageSideLength参数的参照对象。0为按照最长边，1为按照宽度，2为按照高度
        //, maxImageSideLength: 960                    //上传图片最大允许的边长，超过会自动等比缩放,不缩放就设置一个比较大的值，更多设置在image.html中
        , compressSide: 0                            //等比压缩的基准，确定maxImageSideLength参数的参照对象。0为按照最长边，1为按照宽度，2为按照高度
        , maxImageSideLength: 1600                    //上传图片最大允许的边长，超过会自动等比缩放,不缩放就设置一个比较大的值，更多设置在image.html中

        //图片在线管理配置区
        , imageManagerUrl: URL_UPLOAD + "imageManager.ashx"       //图片在线管理的处理地址
        , imageManagerPath: URL_UPLOAD                                    //图片修正地址，同imagePath


        //工具栏上的所有的功能按钮和下拉框，可以在new编辑器的实例时选择自己需要的从新定义
        , toolbars: [
            ['Bold', 'Italic', 'Underline', 'StrikeThrough', 'Superscript', 'Subscript', 'touppercase', 'tolowercase', 'RemoveFormat', 'FormatMatch', 'AutoTypeSet', 'ForeColor', 'BackColor', 'FontFamily', 'FontSize', 'Paragraph', 'CustomStyle', 'RowSpacingTop', 'RowSpacingBottom', 'LineHeight', 'JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyJustify', 'Indent', 'Link', 'Unlink', 'Anchor', 'InsertImage', 'Map', 'PageBreak', 'Horizontal', 'Date', 'Time', 'Spechars', 'InsertOrderedList', 'InsertUnorderedList', 'ImageNone', 'ImageLeft', 'ImageCenter', 'ImageRight', 'InsertTable', 'DeleteTable', 'InsertParagraphBeforeTable', 'InsertRow', 'DeleteRow', 'InsertCol', 'DeleteCol', 'MergeCells', 'MergeRight', 'MergeDown', 'SplittoCells', 'SplittoRows', 'SplittoCols', 'SelectAll', 'ClearDoc', 'SearchReplace', 'Undo', 'Redo', 'Preview', 'Print', 'Source']
        ]
        //Source  隐藏掉改HTML源码, 'FullScreen'
        //'source': '切换源码', 'fullscreen': '全屏'
        //当鼠标放在工具栏上时显示的tooltip提示
        , labelMap: {
            'bold': '加粗', 'italic': '斜体', 'underline': '下划线', 'strikethrough': '删除线', 'superscript': '上标', 'subscript': '下标', 'touppercase': '字母大写', 'tolowercase': '字母小写', 'removeformat': '清除格式', 'formatmatch': '格式刷', 'autotypeset': '自动排版', 'forecolor': '文字颜色', 'backcolor': '背景颜色', 'fontfamily': '字体类型', 'fontsize': '字体大小', 'paragraph': '段落格式', 'customstyle': '自定义样式', 'rowspacingtop': '段前间距', 'rowspacingbottom': '段后间距', 'lineheight': '行间距', 'justifyleft': '居左对齐', 'justifycenter': '居中对齐', 'justifyright': '居右对齐', 'justifyjustify': '两端对齐', 'indent': '首行缩进', 'link': '插入链接', 'unlink': '取消链接', 'anchor': '插入锚点', 'insertimage': '插入图片', 'map': '插入百度地图', 'pagebreak': '插入分页符', 'horizontal': '插入分割线', 'date': '插入日期', 'time': '插入时间', 'spechars': '插入特殊字符', 'insertorderedlist': '插入有序列表', 'insertunorderedlist': '插入无序列表', 'imagenone': '默认', 'imageleft': '图片居左', 'imagecenter': '图片居中', 'imageright': '图片居右', 'inserttable': '插入表格', 'deletetable': '删除表格', 'insertparagraphbeforetable': '表格上方插入行', 'insertrow': '插入行', 'deleterow': '删除行', 'insertcol': '插入列', 'deletecol': '删除列', 'mergecells': '合并单元格', 'mergeright': '向右合并单元格', 'mergedown': '向下合并单元格', 'splittocells': '完全拆分单元格', 'splittorows': '拆分成行', 'splittocols': '拆分成列', 'selectall': '全选', 'cleardoc': '清空文档', 'searchreplace': '查找替换', 'undo': '撤销', 'redo': '重做', 'preview': '预览','print': '打印', 'source': '切换源码，注意保存时请切换回涉及模式！'
        }

        //常用配置项目
        , isShow: true    //默认显示编辑器

        , initialContent: ''    //初始化编辑器的内容,也可以通过textarea/script给值，看官网例子

        , autoClearinitialContent: false //是否自动清除编辑器初始内容，注意：如果focus属性设置为true,这个也为真，那么编辑器一上来就会触发导致初始化的内容看不到了

        , iframeCssUrl: URL + 'themes/default/iframe.css' //给编辑器内部引入一个css文件

        , textarea: 'editorValue' // 提交表单时，服务器获取编辑器提交内容的所用的参数，多实例时可以给容器name属性，会将name给定的值最为每个实例的键值，不用每次实例化的时候都设置这个值

        , focus: false //初始化时，是否让编辑器获得焦点true或false

        , minFrameHeight: "320"  // 最小高度,默认320

        , autoClearEmptyNode: true //getContent时，是否删除空的inlineElement节点（包括嵌套的情况）

        , fullscreen: false //编辑器初始化结束后,编辑区域是否是只读的，默认是false

        , readonly: false //编辑器层级的基数,默认是999

        , zIndex: 999 //图片操作的浮层开关，默认打开，默认是999

        , imagePopup: true

        //,initialStyle:'body{font-size:18px}'   //编辑器内部样式,可以用来改变字体等

        //,emotionLocalization:false //是否开启表情本地化，默认关闭。若要开启请确保emotion文件夹下包含官网提供的images表情文件夹

        //,enterTag:'p' //编辑器回车标签。p或br

        , pasteplain: false  //是否纯文本粘贴。false为不使用纯文本粘贴，true为使用纯文本粘贴

        //iframeUrlMap
        //dialog内容的路径 ～会被替换成URL,垓属性一旦打开，将覆盖所有的dialog的默认路径
        //,iframeUrlMap:{
        // 'anchor':'~/dialogs/anchor/anchor.html',
        // }

        //tab
        //点击tab键时移动的距离,tabSize倍数，tabNode什么字符做为单位
        , tabSize: 4
        , tabNode: '&nbsp;'

        //removeFormat
        //清除格式时可以删除的标签和属性
        //removeForamtTags标签
        //,removeFormatTags:'b,big,code,del,dfn,em,font,i,ins,kbd,q,samp,small,span,strike,strong,sub,sup,tt,u,var'
        //removeFormatAttributes属性
        //,removeFormatAttributes:'class,style,lang,width,height,align,hspace,valign'

        //autotypeset
        //  //自动排版参数
        //  ,autotypeset:{
        //      mergeEmptyline : true,         //合并空行
        //      removeClass : true,           //去掉冗余的class
        //      removeEmptyline : false,      //去掉空行
        //      textAlign : "left" ,           //段落的排版方式，可以是 left,right,center,justify 去掉这个属性表示不执行排版
        //      imageBlockLine : 'center',      //图片的浮动方式，独占一行剧中,左右浮动，默认: center,left,right,none 去掉这个属性表示不执行排版
        //      pasteFilter : false,            //根据规则过滤没事粘贴进来的内容
        //      clearFontSize : false,          //去掉所有的内嵌字号，使用编辑器默认的字号
        //      clearFontFamily : false,        //去掉所有的内嵌字体，使用编辑器默认的字体
        //      removeEmptyNode : false ,       // 去掉空节点
        //      //可以去掉的标签
        //      removeTagNames : {标签名字:1},
        //      indent : false,                 // 行首缩进
        //      indentValue : '2em'             //行首缩进的大小
        //  }


        //pageBreak
        //分页标识符,默认是_baidu_page_break_tag_
        //,pageBreakTag:'_baidu_page_break_tag_'

        //insertorderedlist
        //有序列表的下拉配置
        //,insertorderedlist : {
        //             'decimal' : '' ,         //'1,2,3...'
        //             'lower-alpha' : '' ,    // 'a,b,c...'
        //             'lower-roman' : '' ,    //'i,ii,iii...'
        //             'upper-alpha' : '' ,    //'A,B,C'
        //             'upper-roman' : ''      //'I,II,III...'        
        // }

        //insertunorderedlist
        //无序列表的下拉配置
        //,insertunorderedlist : {
        //    'circle' : '',  // '○ 小圆圈'
        //    'disc' : '',    // '● 小圆点'
        //    'square' : ''   //'■ 小方块'
        //}

        //undo
        //可以最多回退的次数,默认20
         , maxUndoCount: 20
        //当输入的字符数超过该值时，保存一次现场
         , maxInputCount: 20

        //source
        //源码的查看方式,codemirror 是代码高亮，textarea是文本框,默认是codemirror
        //,sourceEditor:"codemirror"
        //如果sourceEditor是codemirror，还用配置一下两个参数
        //codeMirrorJsUrl js加载的路径，默认是 URL + "third-party/codemirror2.15/codemirror.js"
        //,codeMirrorJsUrl:URL + "third-party/codemirror2.15/codemirror.js"
        //codeMirrorCssUrl css加载的路径，默认是 URL + "third-party/codemirror2.15/codemirror.css"
        //,codeMirrorCssUrl:URL + "third-party/codemirror2.15/codemirror.css"

        //autoHeightEnabled
        // 是否自动长高,默认true
        , autoHeightEnabled: false

        //autoFloat
        //是否保持toolbar的位置不动,默认true
        , autoFloatEnabled: true

        //wordCount
        , wordCount: false          //是否开启字数统计
        //,maximumWords:10000       //允许的最大字符数
        , wordCountMsg: ''   //字数统计提示，{#count}代表当前字数，{#leave}代表还可以输入多少字符数
        , wordOverFlowMsg: ''    //超出字数限制提示

        //fontfamily
        //字体
        , 'fontfamily': [{ label: '', name: 'songti', val: '宋体,SimSun' }, { label: '', name: 'kaiti', val: '楷体,楷体_GB2312, SimKai' }, { label: '', name: 'lishu', val: '隶书, SimLi' }, { label: '', name: 'heiti', val: '黑体, SimHei' }, { label: '', name: 'andaleMono', val: 'andale mono' }, { label: '', name: 'arial', val: 'arial, helvetica,sans-serif' }, { label: '', name: 'arialBlack', val: 'arial black,avant garde' }, { label: '', name: 'comicSansMs', val: 'comic sans ms' }, { label: '', name: 'impact', val: 'impact,chicago' }, { label: '', name: 'timesNewRoman', val: 'times new roman' }]

        //fontsize
        //字号
        , 'fontsize': ['10', '11', '12', '14', '16', '18', '20', '24', '36']

        //paragraph
        //段落格式 值:显示的名字
        , 'paragraph': { 'p': '', 'h1': '', 'h2': '', 'h3': '', 'h4': '', 'h5': '', 'h6': '' }

        //customstyle
        //自定义样式
        //block的元素是依据设置段落的逻辑设置的，inline的元素依据BIU的逻辑设置
        //尽量使用一些常用的标签
        //参数说明
        //tag 使用的标签名字
        //label 显示的名字也是用来标识不同类型的标识符，注意这个值每个要不同，
        //style 添加的样式
        //每一个对象就是一个自定义的样式
        , 'customstyle': [{ tag: 'h1', name: 'tc', label: '居中标题', style: 'border-bottom:#ccc 2px solid;padding:0 4px 0 0;text-align:center;margin:0 0 20px 0;' }, { tag: 'h1', name: 'tl', label: '居左标题', style: 'border-bottom:#ccc 2px solid;padding:0 4px 0 0;margin:0 0 10px 0;' }, { tag: 'span', name: 'im', label: '强调', style: 'font-style:italic;font-weight:bold;color:#000' }, { tag: 'span', name: 'hi', label: '明显强调', style: 'font-style:italic;font-weight:bold;color:rgb(51, 153, 204)' }]

        //rowspacingtop
        //段间距 值和显示的名字相同
        , 'rowspacingtop': ['5']

        //rowspacingBottom
        //段间距 值和显示的名字相同
        , 'rowspacingbottom': ['5']

        //lineheight
        //行内间距 值和显示的名字相同
        , 'lineheight': ['1']

    };
})();
