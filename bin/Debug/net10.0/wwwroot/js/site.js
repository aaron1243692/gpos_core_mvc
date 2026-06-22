// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

document.addEventListener("DOMContentLoaded", () => {
  const ribbon = document.querySelector("[data-gpos-ribbon]");
  const userDropdowns = Array.from(document.querySelectorAll(".eg-rb-user"));
  const loginForms = Array.from(document.querySelectorAll(".login-form"));

  loginForms.forEach((form) => {
    const identityInput = form.querySelector(".login-identity-input");
    const passwordInput = form.querySelector(".login-password-input");

    if (identityInput) {
      identityInput.focus();

      identityInput.addEventListener("keydown", (event) => {
        if (event.key === "Enter") {
          event.preventDefault();
          passwordInput?.focus();
        }
      });
    }

    if (passwordInput) {
      passwordInput.addEventListener("keydown", (event) => {
        if (event.key === "Enter") {
          event.preventDefault();
          form.requestSubmit();
        }
      });
    }
  });

  if (userDropdowns.length) {
    document.addEventListener("click", (event) => {
      userDropdowns.forEach((userDropdown) => {
        if (!userDropdown.contains(event.target)) {
          userDropdown.removeAttribute("open");
        }
      });
    });

    document.addEventListener("keydown", (event) => {
      if (event.key === "Escape") {
        userDropdowns.forEach((userDropdown) => {
          if (userDropdown.hasAttribute("open")) {
            userDropdown.removeAttribute("open");
            userDropdown.querySelector("summary")?.focus();
          }
        });
      }
    });
  }

  if (!ribbon) {
    return;
  }

  const headerTabs = Array.from(ribbon.querySelectorAll(".eg-rb-tab"));
  const tabs = Array.from(ribbon.querySelectorAll("[data-gpos-tab]"));
  const panels = Array.from(ribbon.querySelectorAll("[data-gpos-panel]"));
  const pageLinks = Array.from(ribbon.querySelectorAll(".eg-rb-tile"));
  const navDropdowns = Array.from(ribbon.querySelectorAll(".eg-rb-nav-dropdown"));

  const closeHeaderPanels = () => {
    ribbon.classList.remove("is-open");
  };

  const closeDropdownSubmenus = (dropdown = null, exceptGroup = null) => {
    const groups = dropdown
      ? Array.from(dropdown.querySelectorAll(".eg-rb-dropdown-group"))
      : Array.from(ribbon.querySelectorAll(".eg-rb-dropdown-group"));

    groups.forEach((group) => {
      if (group !== exceptGroup) {
        group.classList.remove("is-open");
      }
    });
  };

  const activateHeaderTab = (name, shouldOpen = true) => {
    headerTabs.forEach((tab) => {
      const isActive = tab.dataset.gposTab === name;
      tab.classList.toggle("is-active", isActive);

      if (tab.dataset.gposTab) {
        tab.setAttribute("aria-selected", isActive ? "true" : "false");
      }
    });

    panels.forEach((panel) => {
      panel.classList.toggle("is-active", panel.dataset.gposPanel === name);
    });

    ribbon.classList.toggle("is-open", shouldOpen);
  };

  tabs.forEach((tab) => {
    tab.addEventListener("click", (event) => {
      event.stopPropagation();
      const isSameOpenTab = ribbon.classList.contains("is-open") && tab.classList.contains("is-active");
      activateHeaderTab(tab.dataset.gposTab, !isSameOpenTab);
    });
  });

  navDropdowns.forEach((dropdown) => {
    const toggle = dropdown.querySelector(".eg-rb-tab");
    const groups = Array.from(dropdown.querySelectorAll(".eg-rb-dropdown-group"));

    if (!toggle) {
      return;
    }

    toggle.addEventListener("show.bs.dropdown", () => {
      closeHeaderPanels();
      navDropdowns.forEach((otherDropdown) => {
        if (otherDropdown !== dropdown) {
          closeDropdownSubmenus(otherDropdown);
        }
      });
    });

    toggle.addEventListener("hidden.bs.dropdown", () => {
      closeDropdownSubmenus(dropdown);
    });

    groups.forEach((group) => {
      const groupToggle = group.querySelector(".eg-rb-dropdown-group-toggle");

      if (!groupToggle) {
        return;
      }

      groupToggle.addEventListener("click", (event) => {
        event.preventDefault();
        event.stopPropagation();
        const shouldOpen = !group.classList.contains("is-open");
        closeDropdownSubmenus(dropdown, group);
        group.classList.toggle("is-open", shouldOpen);
      });
    });
  });

  pageLinks.forEach((link) => {
    link.addEventListener("click", () => {
      closeHeaderPanels();
    });
  });

  document.addEventListener("click", (event) => {
    if (!ribbon.contains(event.target)) {
      closeHeaderPanels();
    }
  });

  document.addEventListener("keydown", (event) => {
    if (event.key === "Escape") {
      closeHeaderPanels();
    }
  });

  const enableWheelScroll = (element) => {
    if (!element) {
      return;
    }

    element.addEventListener("wheel", (event) => {
      if (element.scrollWidth <= element.clientWidth) {
        return;
      }

      const delta = Math.abs(event.deltaY) > Math.abs(event.deltaX)
        ? event.deltaY
        : event.deltaX;

      if (!delta) {
        return;
      }

      event.preventDefault();
      element.scrollLeft += delta;
    }, { passive: false });
  };

  enableWheelScroll(ribbon.querySelector(".eg-rb-tabs"));
  enableWheelScroll(ribbon.querySelector(".eg-rb-ribbon"));
});
