// Inside your wwwroot/Design/js/search.js file
document.addEventListener("DOMContentLoaded", function () {

    const searchBox = document.getElementById("searchBox");
    const searchResults = document.getElementById("searchResults");

    // Safety check to ensure the element exists on this specific page
    if (!searchBox) return;

    searchBox.addEventListener("keyup", function () {
        let keyword = this.value;

        if (keyword.length < 1) {
            searchResults.innerHTML = "";
            return;
        }

        fetch(`/Manga/SearchManga?keyword=${keyword}`)
            .then(response => response.json())
            .then(data => {
                let html = "";
                data.forEach(manga => {
                    html += `
                        <a href="/Manga/Details/${manga.mangaId}" class="list-group-item list-group-item-action">
                            <div class="d-flex align-items-center">
                                <img src="${manga.coverImage}" width="50" height="70" class="me-2">
                                <span>${manga.title}</span>
                            </div>
                        </a>`;
                });
                searchResults.innerHTML = html;
            })
            .catch(error => console.error("Error fetching data:", error));
    });
});
