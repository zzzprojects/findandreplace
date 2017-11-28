---
permalink: download
---

<div class="page-download-nuget">
	<div class="container">
		<div class="row justify-content-center">
			<div class="col-lg-6"> 
				<div class="card card-layout-z2 wow slideInLeft">
					<div class="card-header wow slideInDown">
						<h3>
							<a href="{{ site.github.url }}/downloads/fnr.zip" target="_blank"
									onclick="ga('send', 'event', { eventAction: 'download-zip'});">
								Find and Replace Tool
							</a>
						</h3>						
					</div>
					<div class="card-body wow slideInUp">
						<a class="btn btn-xl btn-z wow zoomIn" role="button" href="{{ site.github.url }}/downloads/fnr.zip" target="_blank"
								onclick="ga('send', 'event', { eventAction: 'download-zip'});">
							<i class="fa fa-cloud-download" aria-hidden="true"></i>
							FnR Zip Download
						</a>
						<div class="download-count-text">Download Count:</div>
						<div class="download-count wow lightSpeedIn">
							<a href="{{ site.github.url }}/downloads/fnr.zip" target="_blank"
									onclick="ga('send', 'event', { eventAction: 'download-zip'});">
								<img src="https://zzzprojects.github.io/images/nuget/entity-framework-extensions-big-d.svg">
							</a>
						</div>
					</div>
				</div>
			</div>
		</div>
	</div>
</div>

<style>
.page-download-nuget {
	margin-top: 150px;
}
.page-download-nuget .btn-z {
	margin-bottom: 50px;
}
.page-download-nuget .download-count-text {
	color: #888;
	font-size: 1.25rem;
}
.page-download-nuget .row .col-lg-6 {
	margin-bottom: 60px;
}
@media (max-width: 575px) {
	.page-download-nuget .card-layout-z2 img {
		width: 90%;
	}
	.page-download-nuget .btn-z {
		font-size: 1.5rem;
	}
}
</style>