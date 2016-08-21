$(document).ready(function () {
    
    var $tagEditor = $(".post-tag-editor");

    $tagEditor
        .find(".tag-select") //ul
        .on("click", "> li > a", function (e) {
            e.preventDefault();

            var $this = $(this); //alias out a reference to "this" as a jQuery object
            
            //toggle selector class on the parent li
            var $tagParent = $this.closest("li");
            $tagParent.toggleClass("selected");

            //determine if it's selected
            var selected = $tagParent.hasClass("selected");
            //updates hidden form field value that determines to the server whether or not a particular tag is selected
            $tagParent.find(".selected-input").val(selected);
        });

    var $addTagButton = $tagEditor.find(".add-tag-button");
    var $newTagName = $tagEditor.find(".new-tag-name");

    $addTagButton.click(function (e) {
        e.preventDefault();
        addTag($newTagName.val());
    });

    $newTagName
        .keyup(function (e) {
            //addtag button needs to be enabled or disabled?
            if ($newTagName.val().trim().length > 0)
                $addTagButton.prop("disabled", false);
            else
                $addTagButton.prop("disabled", true);
        })
        .keydown(function (e) {
            //the user hit enter key?
            if (e.which != 13)
                return

            e.preventDefault();
            addTag($newTagName.val());
        });

    function addTag(name) {
        var newIndex = $tagEditor.find(".tag-select > li").length - 1; //next index for count of li tags

        $tagEditor
            .find(".tag-select > li.template") //find template
            .clone()
            .removeClass("template")
            .addClass("selected")
            .find(".name").text(name).end()
            .find(".name-input").attr("name", "Tags[" + newIndex + "].Name").val(name).end()
            .find(".selected-input").attr("name", "Tags[" + newIndex + "].IsChecked").val(true).end()
            .appendTo($tagEditor.find(".tag-select"));

        $newTagName.val("");
        $addTagButton.prop("disabled", true);
    }
});